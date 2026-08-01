namespace ClinicManagement.Domain.Enums;

/// <summary>
/// Represents the stage of a patient's journey with a doctor.
/// Independent of the VisitType/Specialty (Botox, ENT, etc.).
/// A patient builds a timeline: Checkup → Consultation → Procedure over time.
/// </summary>
public enum VisitStage
{
    Checkup = 1,      // كشف — initial examination
    Consultation = 2, // استشارة — follow-up discussion/advice
    Procedure = 3     // عملية / جراحة — medical procedure or surgery
}
