import React from "react";
import { downloadSignedPrescription } from "../apiClient";

type Props = {
  prescriptionId: number;
  label?: string;
};

export const SignedPrescriptionButton: React.FC<Props> = ({
  prescriptionId,
  label = "Download Signed PDF"
}) => {
  const [loading, setLoading] = React.useState(false);

  const handleClick = async () => {
    try {
      setLoading(true);
      const blob = await downloadSignedPrescription(prescriptionId);
      const url = window.URL.createObjectURL(blob);

      const a = document.createElement("a");
      a.href = url;
      a.download = `prescription-${prescriptionId}-signed.pdf`;
      document.body.appendChild(a);
      a.click();
      a.remove();
      window.URL.revokeObjectURL(url);
    } catch (err) {
      console.error(err);
      alert("Could not download signed prescription.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <button
      onClick={handleClick}
      disabled={loading}
      style={{
        padding: "0.6rem 1.2rem",
        borderRadius: "999px",
        border: "none",
        background: loading ? "#94a3b8" : "#2563eb",
        color: "#fff",
        fontWeight: 500,
        cursor: loading ? "not-allowed" : "pointer",
        boxShadow: "0 10px 15px -3px rgba(15, 23, 42, 0.3)"
      }}
    >
      {loading ? "Generating..." : label}
    </button>
  );
};
