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

export interface PatientDocumentDto {
  id: string;
  documentType: string;
  originalFileName: string;
  fileType: string;
  notes?: string;
  createdAt: string;
}

export interface PatientProfileDto {
  demographics: PatientDto;
  totalVisits: number;
  lastVisitDate?: string;
  nextAppointmentDate?: string;
}
