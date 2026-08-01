export interface AppointmentDto {
  id: string;
  patientId: string;
  patientName: string;
  patientPhone?: string;
  
  doctorId: string;
  doctorName: string;
  doctorSpecialty?: string;
  
  createdByEmployeeId?: string;
  
  scheduledStart: string; // ISO DateTime
  scheduledEnd: string;   // ISO DateTime
  
  status: 'Scheduled' | 'Confirmed' | 'CheckedIn' | 'InProgress' | 'Completed' | 'Cancelled' | 'NoShow';
  reason?: string;
  cancellationReason?: string;
  
  visitStage: 'Checkup' | 'Consultation' | 'Procedure';
  visitType?: string;
  
  createdAt: string; // ISO DateTime
}

export interface GetAppointmentsQuery {
  patientName?: string;
  phone?: string;
  doctorId?: string;
  visitType?: string;
  visitStage?: string;
  dateFrom?: string; // ISO DateTime
  dateTo?: string;   // ISO DateTime
  status?: string;
}

export interface CreateAppointmentCommand {
  patientId: string;
  doctorId: string;
  scheduledStart: string; // ISO DateTime
  scheduledEnd: string;   // ISO DateTime
  visitStage: 'Checkup' | 'Consultation' | 'Procedure';
  visitType?: string;
  reason?: string;
}

export interface UpdateAppointmentCommand {
  id: string;
  scheduledStart: string; // ISO DateTime
  scheduledEnd: string;   // ISO DateTime
  status: string;
  visitStage: string;
  visitType?: string;
  reason?: string;
}

export interface CancelAppointmentCommand {
  id: string;
  cancellationReason?: string;
}
