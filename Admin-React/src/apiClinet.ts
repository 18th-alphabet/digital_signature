export async function downloadSignedPrescription(prescriptionId: number) {
  const response = await fetch(`/api/prescriptions/${prescriptionId}/signed-pdf`, {
    method: "GET",
    credentials: "include"
  });

  if (!response.ok) {
    throw new Error(`Failed to download PDF (status ${response.status})`);
  }

  const blob = await response.blob();
  return blob;
}
