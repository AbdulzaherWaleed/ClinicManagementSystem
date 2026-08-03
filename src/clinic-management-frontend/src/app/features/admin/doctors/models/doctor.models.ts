export interface DoctorDto {
  id: string;
  fullName: string;
  title: string | null;
  bio: string | null;
  phoneNumber: string | null;
  email: string | null;
  primarySpecialtyName: string | null;
  isActive: boolean;
  createdAt: string;
}

export interface CreateDoctorCommand {
  fullName: string;
  title?: string;
  bio?: string;
  phoneNumber?: string;
  email?: string;
  primarySpecialtyId?: string;
}

export interface DoctorLicenseDto {
  id: string;
  doctorId: string;
  licenseNumber?: string;
  filePath: string;
  originalFileName: string;
  fileType: string;
  expiryDate?: string;
  isActive: boolean;
}
