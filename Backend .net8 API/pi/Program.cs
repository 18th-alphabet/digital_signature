using Fylde.Prescriptions.Api.Models;
using Fylde.Prescriptions.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddSingleton<PdfSigningService>();
builder.Services.AddScoped<PrescriptionPdfService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS – allow your React admin (adjust origin as needed)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AdminReact",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:5173") // Vite default
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AdminReact");

app.MapGet("/", () => Results.Redirect("/swagger"));

// Fake repo: replace with your DB later
static Prescription FakeLoadPrescription(int id)
{
    return new Prescription
    {
        Id = id,
        PatientName = "Yaqub Hafiz",
        DateIssued = DateTime.UtcNow,
        PrescriberName = "Ahmed Raza",
        ClinicName = "Fylde Clinic",
        ClinicAddress = "Fylde Clinic, Preston PR1 1AA",
        Items =
        {
            new PrescriptionItem
            {
                DrugName = "Omeprazole",
                Strength = "20mg",
                Directions = "Take one capsule daily before breakfast",
                Quantity = 28
            },
            new PrescriptionItem
            {
                DrugName = "Paracetamol",
                Strength = "500mg",
                Directions = "Take 1–2 tablets every 4–6 hours when required",
                Quantity = 32
            }
        }
    };
}

// Get JSON prescription (for debugging / React if needed)
app.MapGet("/api/prescriptions/{id:int}", (int id) =>
{
    var rx = FakeLoadPrescription(id);
    return Results.Ok(rx);
});

// Get signed prescription PDF
app.MapGet("/api/prescriptions/{id:int}/signed-pdf",
    (int id,
     PrescriptionPdfService pdfService,
     PdfSigningService signingService) =>
    {
        var prescription = FakeLoadPrescription(id);
        if (prescription is null)
            return Results.NotFound();

        byte[] unsignedPdf = pdfService.GeneratePrescriptionPdf(prescription);
        byte[] signedPdf = signingService.SignPdf(unsignedPdf);

        var fileName = $"prescription-{id}-signed.pdf";
        return Results.File(signedPdf, "application/pdf", fileName);
    });

app.Run();
