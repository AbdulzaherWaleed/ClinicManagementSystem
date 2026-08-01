export interface EmployeeDto {
  id: string;
  fullName: string;
  phoneNumber: string | null;
  jobTitle: string | null;
  isActive: boolean;
  createdAt: string;
  assignedDoctorIds: string[];
  assignedDoctorNames: string[];
}

export interface RegisterEmployeeCommand {
  fullName: string;
  email: string;
  password: string;
  phoneNumber?: string;
  jobTitle?: string;
  assignedDoctorIds: string[];
}
