using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Models
{
    // Those classes are not DB models.

    public sealed class SchoolComparer : IEqualityComparer<School>
    {
        public static SchoolComparer Default { get; private set; }

        static SchoolComparer()
        {
            Default = new SchoolComparer();
        }

        public bool Equals(School x, School y)
        {
            if (Object.ReferenceEquals(x, y))
                return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.SchoolId.Equals(y.SchoolId);
        }

        public int GetHashCode(School obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return 0;

            return obj.SchoolId.GetHashCode();
        }
    }

    public sealed class AcademicYearComparer : IEqualityComparer<AcademicYear>
    {
        public static AcademicYearComparer Default { get; private set; }

        static AcademicYearComparer()
        {
            Default = new AcademicYearComparer();
        }

        public bool Equals(AcademicYear x, AcademicYear y)
        {
            if (Object.ReferenceEquals(x, y))
                return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.AcademicYearId.Equals(y.AcademicYearId);
        }

        public int GetHashCode(AcademicYear obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return 0;

            return obj.AcademicYearId.GetHashCode();
        }
    }

    public sealed class CourseComparer : IEqualityComparer<Course>
    {
        public static CourseComparer Default { get; private set; }

        static CourseComparer()
        {
            Default = new CourseComparer();
        }

        public bool Equals(Course x, Course y)
        {
            if (Object.ReferenceEquals(x, y))
                return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.CourseId.Equals(y.CourseId);
        }

        public int GetHashCode(Course obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return 0;

            return obj.CourseId.GetHashCode();
        }
    }

    public sealed class ClassComparer : IEqualityComparer<Class>
    {
        public static ClassComparer Default { get; private set; }

        static ClassComparer()
        {
            Default = new ClassComparer();
        }

        public bool Equals(Class x, Class y)
        {
            if (Object.ReferenceEquals(x, y))
                return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.ClassId.Equals(y.ClassId);
        }

        public int GetHashCode(Class obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return 0;

            return obj.ClassId.GetHashCode();
        }
    }

    public sealed class SectionComparer : IEqualityComparer<Section>
    {
        public static SectionComparer Default { get; private set; }

        static SectionComparer()
        {
            Default = new SectionComparer();
        }

        public bool Equals(Section x, Section y)
        {
            if (Object.ReferenceEquals(x, y))
                return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.SectionId.Equals(y.SectionId);
        }

        public int GetHashCode(Section obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return 0;

            return obj.SectionId.GetHashCode();
        }
    }

    public sealed class StudentComparer : IEqualityComparer<Student>
    {
        public static StudentComparer Default { get; private set; }

        static StudentComparer()
        {
            Default = new StudentComparer();
        }

        public bool Equals(Student x, Student y)
        {
            if (Object.ReferenceEquals(x, y))
                return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.StudentId.Equals(y.StudentId);
        }

        public int GetHashCode(Student obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return 0;

            return obj.StudentId.GetHashCode();
        }
    }

    public sealed class StudentSectionComparer : IEqualityComparer<StudentSection>
    {
        public static StudentSectionComparer Default { get; private set; }

        static StudentSectionComparer()
        {
            Default = new StudentSectionComparer();
        }

        public bool Equals(StudentSection x, StudentSection y)
        {
            if (Object.ReferenceEquals(x, y))
                return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.StudentSectionId.Equals(y.StudentSectionId);
        }

        public int GetHashCode(StudentSection obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return 0;

            return obj.StudentSectionId.GetHashCode();
        }
    }

    public sealed class StudentSectionInformationComparer : IEqualityComparer<StudentSectionInfo>
    {
        public static StudentSectionInformationComparer Default { get; private set; }

        static StudentSectionInformationComparer()
        {
            Default = new StudentSectionInformationComparer();
        }

        public bool Equals(StudentSectionInfo x, StudentSectionInfo y)
        {
            if (Object.ReferenceEquals(x, y))
                return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.StudentSection.StudentSectionId.Equals(y.StudentSection.StudentSectionId);
        }

        public int GetHashCode(StudentSectionInfo obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return 0;

            return obj.StudentSection.StudentSectionId.GetHashCode();
        }
    }

    public sealed class GuardianComparer : IEqualityComparer<Guardian>
    {
        public static GuardianComparer Default { get; private set; }

        static GuardianComparer()
        {
            Default = new GuardianComparer();
        }

        public bool Equals(Guardian x, Guardian y)
        {
            if (Object.ReferenceEquals(x, y))
                return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.GuardianId.Equals(y.GuardianId);
        }

        public int GetHashCode(Guardian obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return 0;

            return obj.GuardianId.GetHashCode();
        }
    }
}
