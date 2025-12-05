# Fylde Prescription Signing

Full-stack demo for digitally signed prescription PDFs.

- Backend: .NET 8 Minimal API + iText 7
- Frontend: React (Vite + TypeScript)
- Signature: PKCS#12 (.pfx) digital certificate

## Backend (API)

```bash
cd api
dotnet restore
dotnet run --urls http://localhost:5000
