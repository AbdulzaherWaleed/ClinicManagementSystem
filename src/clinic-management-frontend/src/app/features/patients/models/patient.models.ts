export interface PatientDto {
  id: string;
  fullName: string;
  dateOfBirth?: string;
  gender?: string;
  phoneNumber?: string;
  email?: string;
  address?: string;
  nationalId?: string;
  medicalNotes?: string;
  createdAt: string;
}

export interface CreatePatientCommand {
  fullName: string;
  dateOfBirth?: string;
  gender?: string;
  phoneNumber?: string;
  email?: string;
  address?: string;
  nationalId?: string;
  emergencyContactName?: string;
  emergencyContactPhone?: string;
  medicalNotes?: string;
}

export interface CreatedEntityResponse {
  id: string;
}
