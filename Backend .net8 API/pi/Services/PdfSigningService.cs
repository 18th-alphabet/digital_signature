using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Signatures;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Security;

namespace Fylde.Prescriptions.Api.Services;

public class PdfSigningService
{
    private readonly string _pfxPath;
    private readonly string _pfxPassword;
    private readonly ILogger<PdfSigningService> _logger;

    public PdfSigningService(IConfiguration config, ILogger<PdfSigningService> logger)
    {
        _logger = logger;
        _pfxPath = config["Signing:CertPath"]
            ?? throw new InvalidOperationException("Signing:CertPath is not configured.");
        _pfxPassword = config["Signing:Password"]
            ?? throw new InvalidOperationException("Signing:Password is not configured.");
    }

    public byte[] SignPdf(byte[] unsignedPdfBytes,
                          string reason = "Prescription approval",
                          string location = "Fylde Clinic, UK")
    {
        if (!File.Exists(_pfxPath))
        {
            _logger.LogError("PFX certificate not found at path: {Path}", _pfxPath);
            throw new FileNotFoundException("Signing certificate not found.", _pfxPath);
        }

        using var input = new MemoryStream(unsignedPdfBytes);
        using var output = new MemoryStream();

        var reader = new PdfReader(input);
        var signer = new PdfSigner(reader, output, new StampingProperties());

        // Visible signature appearance
        var appearance = signer.GetSignatureAppearance();
        appearance
            .SetReason(reason)
            .SetLocation(location)
            .SetReuseAppearance(false)
            // bottom-left area on page 1
            .SetPageRect(new Rectangle(36, 36, 250, 50))
            .SetPageNumber(1);

        signer.SetFieldName("Signature1");

        // Load the .pfx / PKCS#12 store
        using var pkcs12Stream = File.OpenRead(_pfxPath);
        var store = new Pkcs12Store(pkcs12Stream, _pfxPassword.ToCharArray());

        var alias = store.Aliases.Cast<string>().FirstOrDefault(store.IsKeyEntry);
        if (alias == null)
        {
            _logger.LogError("No private key entry found in PFX certificate.");
            throw new InvalidOperationException("No private key entry found in PFX certificate.");
        }

        var keyEntry = store.GetKey(alias);
        var privateKey = keyEntry.Key;

        var chainEntries = store.GetCertificateChain(alias);
        X509Certificate[] chain = chainEntries.Select(e => e.Certificate).ToArray();

        IExternalDigest externalDigest = new BouncyCastleDigest();
        IExternalSignature externalSignature =
            new PrivateKeySignature(privateKey, DigestAlgorithms.SHA256);

        signer.SignDetached(
            externalDigest,
            externalSignature,
            chain,
            null,
            null,
            null,
            0,
            PdfSigner.CryptoStandard.CMS
        );

        return output.ToArray();
    }
}
