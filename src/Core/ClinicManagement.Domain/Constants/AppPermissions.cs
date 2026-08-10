namespace ClinicManagement.Domain.Constants;

public static class AppPermissions
{
    // Bookings
    public const string Bookings_Create = "Bookings.Create";
    public const string Bookings_Edit = "Bookings.Edit";
    public const string Bookings_Delete = "Bookings.Delete";
    public const string Bookings_List = "Bookings.List";

    // Doctors
    public const string Doctors_Create = "Doctors.Create";
    public const string Doctors_Edit = "Doctors.Edit";
    public const string Doctors_Delete = "Doctors.Delete";
    public const string Doctors_List = "Doctors.List";

    // Reports
    public const string Reports_Create = "Reports.Create"; // Might not be needed, but good for completeness
    public const string Reports_Edit = "Reports.Edit";
    public const string Reports_Delete = "Reports.Delete";
    public const string Reports_List = "Reports.List";

    // Licenses
    public const string Licenses_Create = "Licenses.Create";
    public const string Licenses_Edit = "Licenses.Edit";
    public const string Licenses_Delete = "Licenses.Delete";
    public const string Licenses_List = "Licenses.List";

    // Patients
    public const string Patients_Create = "Patients.Create";
    public const string Patients_Edit = "Patients.Edit";
    public const string Patients_Delete = "Patients.Delete";
    public const string Patients_List = "Patients.List";

    // Employees
    public const string Employees_Create = "Employees.Create";
    public const string Employees_Edit = "Employees.Edit";
    public const string Employees_Delete = "Employees.Delete";
    public const string Employees_List = "Employees.List";

    public static IReadOnlyList<string> GetAll()
    {
        return typeof(AppPermissions)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.FlattenHierarchy)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
            .Select(x => (string)x.GetRawConstantValue()!)
            .ToList();
    }
}
