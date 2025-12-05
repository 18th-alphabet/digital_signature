namespace Fylde.Prescriptions.Api.Models;

public class Prescription
{
    public int Id { get; set; }
    public string PatientName { get; set; } = default!;
    public DateTime DateIssued { get; set; }
    public string PrescriberName { get; set; } = default!;
    public string ClinicName { get; set; } = default!;
    public string ClinicAddress { get; set; } = default!;
    public List<PrescriptionItem> Items { get; set; } = new();
}

public class PrescriptionItem
{
    public string DrugName { get; set; } = default!;
    public string Strength { get; set; } = default!;
    public string Directions { get; set; } = default!;
    public int Quantity { get; set; }
}