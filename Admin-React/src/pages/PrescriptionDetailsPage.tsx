import React from "react";
import { SignedPrescriptionButton } from "../components/SignedPrescriptionButton";

export const PrescriptionDetailsPage: React.FC = () => {
  const prescriptionId = 18523; // swap for real ID / route param later

  return (
    <div
      style={{
        minHeight: "100vh",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        background: "#0f172a"
      }}
    >
      <div
        style={{
          background: "white",
          borderRadius: "1.5rem",
          padding: "2rem 3rem",
          boxShadow: "0 25px 50px -12px rgba(15, 23, 42, 0.6)",
          maxWidth: "520px",
          width: "100%"
        }}
      >
        <h1 style={{ fontSize: "1.5rem", marginBottom: "0.5rem" }}>
          Prescription #{prescriptionId}
        </h1>
        <p style={{ marginBottom: "1.5rem", color: "#64748b" }}>
          Generate a digitally signed PDF prescription using the clinic’s
          signing certificate.
        </p>

        <SignedPrescriptionButton prescriptionId={prescriptionId} />

        <p
          style={{
            marginTop: "1rem",
            fontSize: "0.8rem",
            color: "#94a3b8"
          }}
        >
          The PDF will be cryptographically signed (PKCS#7) and will show as
          digitally signed in Adobe Reader.
        </p>
      </div>
    </div>
  );
};
