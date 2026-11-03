using CodeflexSystems.Library.Core;
using CodeflexSystems.Library.Expressions;
using CodeflexSystems.Library.Extensions;
using CodeflexSystems.Library.Reflection;
using SMS.Data;
using SMS.Extensions;
using SMS.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TDD
{
    #region classes

    public class Product
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.ProductName))
                return base.ToString();
            return this.ProductName;
        }
    }

    public class Item
    {
        public Guid ItemId { get; set; }
        public string ItemName { get; set; }

        public override bool Equals(object obj)
        {
            var value = obj as Item;

            if (Object.ReferenceEquals(this, value))
                return true;

            if (Object.ReferenceEquals(this, null) || Object.ReferenceEquals(value, null))
                return false;

            return this.ItemId.Equals(value.ItemId) && this.ItemName == value.ItemName;
        }

        public override int GetHashCode()
        {
            if (Object.ReferenceEquals(this, null))
                return 0;

            return this.ItemId.GetHashCode() ^ (this.ItemName == null ? 0 : this.ItemName.GetHashCode());
        }
    }

    public sealed class PhoneGenericComparer<T> : IEqualityComparer<T>
        where T : Phone
    {
        public static PhoneGenericComparer<T> Default { get; private set; }

        static PhoneGenericComparer()
        {
            Default = new PhoneGenericComparer<T>();
        }

        public bool Equals(T x, T y)
        {
            if (Object.ReferenceEquals(x, y))
                return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.PhoneId.Equals(y.PhoneId);
        }

        public int GetHashCode(T obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return 0;

            return obj.PhoneId.GetHashCode();
        }
    }

    public sealed class PhoneNonGenericComparer : IEqualityComparer<Phone>
    {
        public static PhoneNonGenericComparer Default { get; private set; }

        static PhoneNonGenericComparer()
        {
            Default = new PhoneNonGenericComparer();
        }

        public bool Equals(Phone x, Phone y)
        {
            if (Object.ReferenceEquals(x, y))
                return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.PhoneId.Equals(y.PhoneId);
        }

        public int GetHashCode(Phone obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return 0;

            return obj.PhoneId.GetHashCode();
        }
    }

    public sealed class SectionGenericComparer<T> : IEqualityComparer<T>
        where T : Section
    {
        public static SectionGenericComparer<T> Default { get; private set; }

        static SectionGenericComparer()
        {
            Default = new SectionGenericComparer<T>();
        }

        public bool Equals(T x, T y)
        {
            if (Object.ReferenceEquals(x, y))
                return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.SectionId.Equals(y.SectionId);
        }

        public int GetHashCode(T obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return 0;

            return obj.SectionId.GetHashCode();
        }
    }

    public sealed class SectionNonGenericComparer : IEqualityComparer<Section>
    {
        public static SectionNonGenericComparer Default { get; private set; }

        static SectionNonGenericComparer()
        {
            Default = new SectionNonGenericComparer();
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

    public static class Extensions
    {
        public static EntityType GetCSpaceEntityType(this MetadataWorkspace workspace, Type type)
        {
            EntityType ospaceEntityType = null;
            StructuralType cspaceEntityType = null;
            EntityType entityType = null;
            if (workspace.TryGetItem<EntityType>(
                ObjectContext.GetObjectType(type).FullName,
                DataSpace.OSpace,
                out ospaceEntityType))
            {
                if (workspace.TryGetEdmSpaceType(
                    ospaceEntityType,
                    out cspaceEntityType))
                {
                    entityType = cspaceEntityType as EntityType;
                }
            }
            if (entityType == null)
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "Unable to find a CSpace type for type {0}", type.FullName));
            }
            return entityType;
        }

        public static object GetValue(this EntityReference entityReference)
        {
            foreach (object value in entityReference)
            {
                return value;
            }
            return null;
        }

        private static string CreateReferenceKeyLookup(string keyMemberName, EntityReference reference, NavigationProperty navigationProperty)
        {
            // use the more usable navigation property name to qualify the member
            // if available
            if (navigationProperty != null)
            {
                return String.Format(CultureInfo.InvariantCulture, "{0}.{1}", navigationProperty.Name, keyMemberName);
            }
            else
            {
                return String.Format(CultureInfo.InvariantCulture, "Navigate({0}.{1}).{2}", reference.RelationshipSet.ElementType.FullName, reference.TargetRoleName, keyMemberName);
            }
        }

        private static IEnumerable<EntityReference> EnumerateSaveReferences(RelationshipManager manager)
        {
            return manager.GetAllRelatedEnds().OfType<EntityReference>()
                    .Where(er => er.RelationshipSet.ElementType.RelationshipEndMembers[er.SourceRoleName].RelationshipMultiplicity != RelationshipMultiplicity.One &&
                        !((AssociationSet)er.RelationshipSet).ElementType.IsForeignKey);
        }

        internal static void StoreReferenceKeyValues<T>(this ObjectContext context, T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            ObjectStateEntry entry;
            if (!context.ObjectStateManager.TryGetObjectStateEntry(entity, out entry))
            {
                // must be a no tracking query, the reference key info won't be available
                return;
            }

            var relationshipManager = entry.RelationshipManager;
            EntityType entityType = context.MetadataWorkspace.GetCSpaceEntityType(entity.GetType());
            foreach (EntityReference entityReference in EnumerateSaveReferences(relationshipManager))
            {
                NavigationProperty navigationProperty = entityType.NavigationProperties.FirstOrDefault(n => n.RelationshipType == entityReference.RelationshipSet.ElementType &&
                        n.FromEndMember.Name == entityReference.SourceRoleName &&
                        n.ToEndMember.Name == entityReference.TargetRoleName);

                object value = entityReference.GetValue();
                if ((navigationProperty == null || value == null) && entityReference.EntityKey != null)
                {
                    foreach (var item in entityReference.EntityKey.EntityKeyValues)
                    {
                        string key = CreateReferenceKeyLookup(item.Key, entityReference, navigationProperty);
                        //entity.ChangeTracker.ExtendedProperties.Add(key, item.Value);
                        Console.WriteLine(key);
                    }
                }
            }
        }

    }

    #endregion

    class Program
    {
        static void Main(string[] args)
        {
            //Test11();
            //Test12();
            Test13();
            //Test04("AE63C341-5A52-E611-B3BD-C05013172761");
            //Test04("EDDA758A-C3DA-E411-B7D6-FF0411E1DDD9");

            Console.WriteLine();
            Console.WriteLine("Press any keys ...");
            Console.ReadKey();
        }

        static void Test04(string id)
        {
            using (var sms = new SMSUnitOfWork())
            {
                var uo = new UserOption
                {
                    UserId = Guid.Parse(id),
                    AccentColorA = 255,
                    AccentColorR = 111,
                    AccentColorG = 130,
                    AccentColorB = 173,
                    TransitionEffect = 4,
                    PageSize = 20,
                    DatePattern = "d/M/yyyy",
                    TimePattern = "h:mm tt",
                    DateTimeIndex = 1,
                    CurrencyPositivePattern = 3,
                    CurrencyNegativePattern = 8
                };

                //var s1 = sms.Entry(uo).State;
                //sms.Attach(uo);
                //var s2 = sms.Entry(uo).State;
                //Console.WriteLine(s1);
                //Console.WriteLine(s2);
                //Console.WriteLine();
            }
        }

        static void Test11()
        {
            using (var sms = new SMSUnitOfWork())
            {
                var q1 = from o in sms.Context.Schools
                         from y in o.AcademicYears.Where(m => m.Active)
                         from r in y.Courses
                         from c in r.Classes
                         from s in c.Sections
                         from x in s.StudentSections
                         select x;
                var q2 = from x in q1
                         select x.Student;
                var q3 = from t in q2
                         select t.StudentId;

                Console.WriteLine("Student Count - All");
                Console.WriteLine("Student Sections : {0}", q1.Count());
                Console.WriteLine("Total Students : {0}", q2.Count());
                Console.WriteLine("Distinct Students : {0}", q3.Distinct().Count());
                Console.WriteLine("Change Tracker Count : {0}", sms.Context.ChangeTracker.Entries().Count());
                Console.WriteLine();
            }
        }

        static void Test12()
        {
            using (var sms = new SMSUnitOfWork())
            {
                var q1 = from o in sms.Context.Schools
                         from y in o.AcademicYears.Where(m => m.Active)
                         from r in y.Courses
                         from c in r.Classes
                         from s in c.Sections
                         from x in s.StudentSections.Where(m => m.Active)
                         select x;
                var q2 = from x in q1
                         select x.Student;
                var q3 = from t in q2
                         select t.StudentId;

                Console.WriteLine("Student Count - Active");
                Console.WriteLine("Student Sections : {0}", q1.Count());
                Console.WriteLine("Total Students : {0}", q2.Count());
                Console.WriteLine("Distinct Students : {0}", q3.Distinct().Count());
                Console.WriteLine("Change Tracker Count : {0}", sms.Context.ChangeTracker.Entries().Count());
                Console.WriteLine();
            }
        }

        static void Test13()
        {
            using (var sms = new SMSUnitOfWork())
            {
                var q1 = from o in sms.Context.Schools
                         from y in o.AcademicYears.Where(m => m.Active)
                         from r in y.Courses
                         from c in r.Classes
                         from s in c.Sections
                         from x in s.StudentSections.Where(m => m.Active)
                         select x.StudentId;

                Console.WriteLine("Student Count - Active");
                Console.WriteLine("Distinct Students : {0}", q1.Distinct().Count());
                Console.WriteLine("Change Tracker Count : {0}", sms.Context.ChangeTracker.Entries().Count());
                Console.WriteLine();
            }
        }

        static void Test03()
        {
            using (var sms = new SMSUnitOfWork())
            {
                var l1 = sms.Context.Currencies.Take(5)
                    .OrderBy(m => m.CreatedDate)
                    .ToObservableCollection();
                var l2 = sms.Context.Currencies.Take(5)
                    .OrderBy(m => m.CreatedDate).OrderBy(m => m.CurrencyName)
                    .ToObservableCollection();
                var l3 = l1.OrderBy(m => m.CurrencyName).ToObservableCollection();

                Print(l1);
                Print(l2);
                Print(l3);
            }
        }

        static void Print(IEnumerable<Currency> source)
        {
            foreach (var v in source)
                Console.WriteLine(v.CurrencyName);
            Console.WriteLine();
        }

        static void Test01()
        {
            //using (var sms = new SMSUnitOfWork())
            //{
            //    string pn = "\u0061\u030a";
            //    bool flag = sms.Any<SchoolPhone>(m => pn == m.PhoneNumber);

            //    Console.WriteLine(flag);
            //}
            using (var sms = new SMSUnitOfWork())
            {
                string pn = "\u0061\u030a";
                bool flag = sms.Any<SchoolPhone>(m =>
                    string.Compare(pn, m.PhoneNumber, StringComparison.InvariantCultureIgnoreCase) == 0);

                Console.WriteLine(flag);
            }

            //using (var sms = new SMSUnitOfWork())
            //{
            //    string un = "or";
            //    bool flag = sms.Context.Set<User>().Any(m =>
            //        m.UserName.Contains(un));
            //    Console.WriteLine(flag);

            //    bool flag2 = sms.Context.Set<User>().Any(m =>
            //        un.Contains(m.UserName));
            //    Console.WriteLine(flag2);
            //}

            //using (var sms = new SMSUnitOfWork())
            //{
            //    string pn = "09199991";
            //    string ex = null;
            //    var list = sms.SelectAllAsNoTracking<SchoolPhone>().ToList();
            //    bool flag = list.Any(m =>
            //        string.Equals(ex, m.Extension, StringComparison.InvariantCultureIgnoreCase) &&
            //        string.Equals(pn, m.PhoneNumber, StringComparison.InvariantCultureIgnoreCase));

            //    Console.WriteLine(flag);
            //    ShowChangeTracker(sms);
            //}

            //using (var sms = new SMSUnitOfWork())
            //{
            //    string pn = "09199991";
            //    string ex = null;
            //    bool flag = sms.Where<SchoolPhone>(m =>
            //        ex.Equals(m.Extension, StringComparison.InvariantCultureIgnoreCase) &&
            //        pn.Equals(m.PhoneNumber, StringComparison.InvariantCultureIgnoreCase)).Any();

            //    Console.WriteLine(flag);
            //    ShowChangeTracker(sms);
            //}

            //using (var sms = new SMSUnitOfWork())
            //{
            //    //var id1 = Guid.Parse("A793D336-E6DA-E411-B7D6-FF0411E1DDD9");
            //    var id1 = Guid.Parse("A893D336-E6DA-E411-B7D6-FF0411E1DDD9");
            //    var g1 = sms.FirstOrDefault<Guardian>(m => m.GuardianId == id1);
            //    var c1 = sms.FirstOrDefault<Country>();

            //    Console.WriteLine(g1.BirthCountry);
            //    PrintChangeTracker(sms);

            //    g1.BirthCountry = c1;
            //    Console.WriteLine(g1.BirthCountry);
            //    PrintChangeTracker(sms);

            //    sms.RefreshToOriginalValues(g1);
            //    Console.WriteLine(g1.BirthCountry);
            //    PrintChangeTracker(sms);
            //}

            //using (var sms = new SMSUnitOfWork())
            //{
            //    //var id1 = Guid.Parse("A793D336-E6DA-E411-B7D6-FF0411E1DDD9");
            //    var id1 = Guid.Parse("A893D336-E6DA-E411-B7D6-FF0411E1DDD9");
            //    var g1 = sms.FirstOrDefault<Guardian>(m => m.GuardianId == id1);
            //    var c1 = sms.FirstOrDefault<Religion>();

            //    Console.WriteLine(g1.Religion);
            //    PrintChangeTracker(sms);

            //    g1.Religion = c1;
            //    Console.WriteLine(g1.Religion);
            //    PrintChangeTracker(sms);

            //    sms.RefreshToOriginalValues(g1);
            //    Console.WriteLine(g1.Religion);
            //    PrintChangeTracker(sms);
            //}

            //using (var sms = new SMSUnitOfWork())
            //{
            //    var id1 = Guid.Parse("A793D336-E6DA-E411-B7D6-FF0411E1DDD9");
            //    //var id1 = Guid.Parse("A893D336-E6DA-E411-B7D6-FF0411E1DDD9");
            //    var g1 = sms.FirstOrDefault<Guardian>(m => m.GuardianId == id1);

            //    if (g1.Photo == null)
            //    {
            //        g1.Photo = new GuardianPhoto();
            //        g1.Photo.Photo = GetPhotoBytes(@"d:\IMG_20130908_085005_edited.jpg");
            //    }
            //    g1.Photo.Photo = null;
            //    PrintChangeTracker(sms);

            //    sms.RefreshToOriginalValues(g1);
            //    if (sms.Entry(g1.Photo).State == EntityState.Added)
            //        g1.Photo = null;
            //    else
            //        sms.RefreshToOriginalValues(g1.Photo);
            //    PrintChangeTracker(sms);
            //}
        }

        static byte[] GetPhotoBytes()
        {
            return new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 };
        }

        static byte[] GetPhotoBytes(string path)
        {
            var fi = new FileInfo(path);

            if (fi.Exists)
            {
                byte[] bs = new byte[fi.Length];

                using (var s = File.OpenRead(path))
                {
                    s.Read(bs, 0, (int)fi.Length);
                    s.Close();
                }

                return bs;
            }

            return null;
        }

        static void InvalidReceipts()
        {
            using (var cox = new SMSContext())
            {
                var list = new List<string>();

                foreach (var r in cox.Receipts)
                {
                    list.Clear();
                    if (r.TotalAmount != r.ComputeTotalAmount())
                        list.Add("Invalid TotalAmount");
                    if (r.NetAmount != r.ComputeNetAmount())
                        list.Add("Invalid NetAmount");
                    foreach (var d in r.ReceiptDetails)
                    {
                        if (d.NetAmount != d.ComputeNetAmount())
                            list.Add("Invalid Detail NetAmount");
                    }
                    if (list.Count > 0)
                    {
                        foreach (var error in list)
                            Console.WriteLine(error);
                        Console.WriteLine();
                        Console.WriteLine("Receipt : {0}", r.ReceiptCodeNo);
                        Console.WriteLine("Date : {0}", r.ReceiptDate.ToShortDateString());
                        Console.WriteLine("TotalAmount : {0}", r.TotalAmount);
                        Console.WriteLine("TaxAmount : {0}", r.TaxAmount);
                        Console.WriteLine("NetAmount : {0}", r.NetAmount);
                        foreach (var d in r.ReceiptDetails)
                        {
                            Console.WriteLine();
                            Console.WriteLine("TotalAmount : {0}", d.ComputeTotalAmount());
                            Console.WriteLine("DiscountAmount : {0}", d.DiscountAmount);
                            Console.WriteLine("NetAmount : {0}", d.NetAmount);
                        }
                        Console.WriteLine("========");
                        Console.WriteLine();
                    }
                }
            }
        }

        static void RepairReceiptDates()
        {
            using (var cox = new SMSContext())
            {
                foreach (var r in cox.Receipts)
                {
                    if (r.ReceiptDate != r.CreatedDate.Date)
                        r.CreatedDate = new DateTime(r.ReceiptDate.Ticks + r.CreatedDate.TimeOfDay.Ticks);
                    if (r.ReceiptDate != r.ModifiedDate.Date)
                        r.ModifiedDate = new DateTime(r.ReceiptDate.Ticks + r.ModifiedDate.TimeOfDay.Ticks);
                    //Console.WriteLine(r.CreatedDate);
                    //Console.WriteLine(r.ModifiedDate);
                    //Console.WriteLine();
                }
                cox.SaveChanges();
            }
        }

        static void Test24()
        {
            //using (UnitOfWork uow = new UnitOfWork())
            //{
            //    uow.Configuration.ProxyCreationEnabled = false;
            //    var students = uow.WhereAsNoTracking<Student>(m => m.BirthDate.HasValue).Take(10).ToList();

            //    var now = DateTime.Today;
            //    foreach (var value in students)
            //    {
            //        var age = now - value.BirthDate.Value;
            //        var ageym = new DateTime(age.Ticks);
            //        Console.WriteLine("{0}, {1,30}, {2}, {3,5}, {4,2}, {5,2}",
            //            value.Id, value.StudentName, value.BirthDate.Value.ToString("MM/dd/yyyy"),
            //            age.Days, ageym.Year, ageym.Month);
            //    }
            //}

            //var dob = new DateTime(1978, 10, 26);
            //var td = new DateTime(2014, 10, 25);

            //Console.WriteLine(dob.ToShortDateString());
            //Console.WriteLine();

            //for (int i = 0; i < 12; i++)
            //{
            //    td = td.AddMonths(1);
            //    var ts =  td - dob;
            //    var dd = new DateTime(ts.Ticks);
            //    var age = ts.TotalDays / 365.25;
            //    var y = Math.Floor(age);
            //    var m = Math.Floor((age - y) * 12);

            //    Console.WriteLine("{0}, {1}, {2}/{3}, {4}", td.ToShortDateString(),
            //        dd.ToShortDateString(), m, y, age);
            //    Console.WriteLine();
            //}

            using (UnitOfWork uow = new UnitOfWork())
            {
                uow.Configuration.ProxyCreationEnabled = false;
                var students = uow.WhereAsNoTracking<Student>(m => m.BirthDate.HasValue)
                    .OrderBy(m => m.Id).ToList();

                var now = DateTime.Today;
                int i = 0;
                foreach (var value in students)
                {
                    var age = now - value.BirthDate.Value;
                    var ageym = new DateTime(age.Ticks);
                    var newage = new DateTime(age.Ticks);

                    //if (ageym.Day != -1)
                    newage = newage.AddDays(-1);
                    //if (newage.Month != 1)
                    newage = newage.AddMonths(-1);
                    newage = newage.AddYears(-1);

                    var sage = age.TotalDays / 365.25;
                    var sy = Math.Floor(sage);
                    var sm = Math.Floor((sage - sy) * 12);

                    //if ((ageym.Year - 1 != sy) || (ageym.Month - 1 != sm))
                    //if (ageym.Year - 2 == sy)
                    //if (newage.Year != sy || newage.Month != sm)
                    if ((ageym.Year - 1 != newage.Year) || (ageym.Month - 1 != newage.Month))
                    {
                        //Console.WriteLine("{0}, {1}, {2}, {3,2}, {4,2}, {5,2}, {6,2}, {7}, {8}",
                        //    (++i).ToString().PadLeft(3), value.Id, value.BirthDate.Value.ToString("yyyy/MM/dd"),
                        //    sy, sm, ageym.Year - 1, ageym.Month - 1,
                        //    ageym.ToString("yy/MM/dd"), newage.ToString("yy/MM/dd"));
                        Console.WriteLine(@"{0}, {1}, {2}, {3}/{4}, {5}/{6}, {7}, {8}",
                            (++i).ToString().PadLeft(3), value.Id, value.BirthDate.Value.ToString("yyyy/MM/dd"),
                            ToString(sy), ToString(sm), ToString(ageym.Year - 1), ToString(ageym.Month - 1),
                            ageym.ToString("yy/MM/dd"), newage.ToString("yy/MM/dd"));
                    }
                }
            }

            //var dt = DateTime.Today;
            //var dt2 = dt.AddDays(-1);
            //var diff = new DateTime((dt - dt2).Ticks);

            //Console.WriteLine(diff.ToString("yy/MM/dd"));
        }

        static string ToString(double value)
        {
            return value.ToString().PadLeft(2, '0');
        }

        static string ToString(int value)
        {
            return value.ToString().PadLeft(2, '0');
        }

        static void Test25()
        {
            using (var sms = new SMSUnitOfWork())
            {
                var sid = new Guid("6b2ff31c-f3da-e411-b7d6-ff0411e1ddd9");
                var name = "Myo Min";

                var query = sms.Where<StudentSection>(m => m.Student.StudentName.Contains(name));
                query = from t in query
                        where t.Section.Class.Course.AcademicYear.School.SchoolId == sid
                        select t;
                //query = from t in query
                //        where t.Section.Class.Course.AcademicYear.IsActive == true
                //        select t;
                //var list = query.Include(m => m.Student).Include(m => m.Section.Class.Course).ToList();
                var list = (from t in query
                            select new StudentSectionInfo { Student = t.Student, Section = t.Section, Class = t.Section.Class, Course = t.Section.Class.Course, AcademicYear = t.Section.Class.Course.AcademicYear }).AsNoTracking().ToList();

                Console.WriteLine("Count = {0}", list.Count);
                Console.WriteLine("Tracker Count = {0}", sms.Context.ChangeTracker.Entries().Count());
            }
        }

        static void Test26()
        {
            using (var sms = new SMSUnitOfWork())
            {
                var filterTypes = new Type[] { typeof(SchoolEmail), typeof(SchoolPhone) };
                var filterStates = new EntityState[] { EntityState.Added, EntityState.Modified, EntityState.Deleted };

                var item1 = sms.GetSchools(m => m.SchoolName == "School 3")
                    .Include(m => m.Address)
                    .Include(m => m.SchoolEmails)
                    .Include(m => m.SchoolPhones)
                    .First();

                PrintChangeTracker(sms, false, null, true, filterStates);
                Console.WriteLine();

                //item1.Address.Line2 = "line2";

                //item1.Address = new Address();

                //PrintChangeTracker(sms, false, filterTypes, true, filterStates);
                //Console.WriteLine();

                item1.Address = sms.Context.Addresses.OrderBy(m => m.Line1).Skip(10).Take(1).First();

                //PrintChangeTracker(sms, false, filterTypes, true, filterStates);
                //Console.WriteLine();

                //item1.SchoolEmails.Add(new SchoolEmail { EmailAddress = "s3@gmail.com" });
                //sms.Context.SchoolPhones.Remove(item1.SchoolPhones.First());

                PrintChangeTracker(sms, false, null, true, filterStates);
                Console.WriteLine();

                sms.Clear();
            }
        }

        static void Test02()
        {
            using (var sms = new SMSUnitOfWork())
            {
                var filterTypes = new Type[] { typeof(Country), typeof(PhoneType) };
                var filterStates = new EntityState[] { EntityState.Added, EntityState.Modified, EntityState.Deleted };

                var item = sms.GetSchools(m => m.SchoolName == "School 3")
                    .Include(m => m.Address).Include(m => m.SchoolEmails)
                    .Include(m => m.SchoolPhones).First();

                PrintChangeTracker(sms, false, filterTypes, true, filterStates);
                Console.WriteLine();

                item.Address = sms.Context.Addresses.First();

                var phone = new SchoolPhone { PhoneNumber = "09987456321", PhoneType = sms.PhoneTypes.First() };
                item.SchoolPhones.Add(phone);
                item.SchoolPhones.First().PhoneType = sms.PhoneTypes.First();

                PrintChangeTracker(sms, false, filterTypes, true, filterStates);
                Console.WriteLine();

                sms.RefreshToOriginalValues(item);
                //sms.RefreshRange(item.SchoolPhones);
                sms.RefreshRangeToOriginalValues(item.SchoolPhones);

                PrintChangeTracker(sms, false, filterTypes, true, filterStates);
                Console.WriteLine();

                sms.Clear();
            }
        }

        static void Test05()
        {
            using (var sms = new SMSUnitOfWork())
            {
                var id = new Guid("6B2FF31C-F3DA-E411-B7D6-FF0411E1DDD9");
                var filterStates = new EntityState[] { EntityState.Added, EntityState.Modified, EntityState.Deleted };

                var query1 = sms.GetStudentSections(m => m.Student.StudentName.Contains("Aung"), id, true);
                var query2 = sms.GetStudentSections(m => m.Student.OtherName.Contains("Aung"), id, true);

                var list1 = query1.ToList();
                var list2 = query2.ToList();
                var query3 = from t in list1.Intersect(list2, StudentSectionComparer.Default)
                             select new StudentSectionInfo
                             {
                                 StudentSection = t,
                                 Student = t.Student,
                                 Section = t.Section,
                                 Class = t.Section.Class,
                                 Course = t.Section.Class.Course,
                                 AcademicYear = t.Section.Class.Course.AcademicYear
                             };
                var list3 = query3.ToList();

                //PrintChangeTracker(sms, false, null, true, filterStates);
                Console.WriteLine(list1.Count);
                Console.WriteLine(list2.Count);
                Console.WriteLine(list3.Count);

                foreach (var value in list3)
                    Console.WriteLine(value.Student.StudentFullName);
            }
        }

        static void Test06()
        {
            using (var sms = new SMSUnitOfWork())
            {
                var text = "Aung Aung";
                var id = new Guid("6B2FF31C-F3DA-E411-B7D6-FF0411E1DDD9");
                var query = sms.GetStudentSectionInfosByName(text, id, true);
                var list = query.ToList();

                Console.WriteLine("Search Student Sections of {0}", text);
                Console.WriteLine("Count : {0}", list.Count);
                foreach (var value in list)
                    Console.WriteLine("{0}, {1}", value.Student.Id, value.Student.StudentFullName);
                Console.WriteLine();
            }
        }

        static void Test07()
        {
            using (var sms = new SMSUnitOfWork())
            {
                var text = "Aung Aung";
                var id = new Guid("6B2FF31C-F3DA-E411-B7D6-FF0411E1DDD9");
                var param = ExpressionHelper.CreateParameter<Student>();
                var left = ExpressionHelper.CreateExpression<Student>(
                    "StudentName", text, StringMatchType.Contains, param);
                var right = ExpressionHelper.CreateExpression<Student>(
                    "OtherName", text, StringMatchType.Contains, param);
                var query = sms.Where(ExpressionHelper.CreatePredicate<Student>(
                    ExpressionType.OrElse, left, right, param)).SelectMany(t => t.StudentSections);
                var query2 = from t in query.Where(m =>
                                 m.Section.Class.Course.AcademicYear.SchoolId == id &&
                                 m.Section.Class.Course.AcademicYear.Active == true)
                             select new StudentSectionInfo
                             {
                                 StudentSection = t,
                                 Student = t.Student,
                                 Section = t.Section,
                                 Class = t.Section.Class,
                                 Course = t.Section.Class.Course,
                                 AcademicYear = t.Section.Class.Course.AcademicYear,
                                 School = t.Section.Class.Course.AcademicYear.School
                             };
                var list = query2.ToList();

                Console.WriteLine("Search Student Sections of {0}", text);
                Console.WriteLine("Count : {0}", list.Count);
                foreach (var value in list)
                    Console.WriteLine("{0}, {1}", value.Student.Id, value.Student.StudentFullName);
                Console.WriteLine();

            }
        }

        static void Test08()
        {
            using (var sms = new SMSUnitOfWork())
            {
                //var text = "Aung Myo Thu";
                var text = "aung aung";
                var query = sms.Where<StudentFerryFee>(m =>
                        m.StudentSection.Student.StudentName.Contains(text) ||
                        m.StudentSection.Student.OtherName.Contains(text))
                        .Select(m => m.StudentSection.Student);
                var list = query.ToList();

                Console.WriteLine("Search Student Ferry Fee of {0}", text);
                Console.WriteLine("Count : {0}", list.Count);
                foreach (var value in list)
                    Console.WriteLine("{0}, {1}", value.Id, value.StudentFullName);
                Console.WriteLine();
            }
        }

        static void Test09()
        {
            using (var sms = new SMSUnitOfWork())
            {
                //var text = "Aung Myo Thu";
                var text = "aung aung";
                var param = ExpressionHelper.CreateParameter<Student>();
                var left = ExpressionHelper.CreateExpression<Student>(
                    "StudentName", text, StringMatchType.Contains, param);
                var right = ExpressionHelper.CreateExpression<Student>(
                    "OtherName", text, StringMatchType.Contains, param);
                var query = sms.Where(ExpressionHelper.CreatePredicate<Student>(
                    ExpressionType.OrElse, left, right, param));
                var query2 = from t in query.SelectMany(m => m.StudentSections)
                             where t.StudentFerryFee != null
                             select t.Student;
                var list = query2.OrderBy(m => m.StudentName).ThenBy(m => m.OtherName).ToList();

                Console.WriteLine("Search Student Ferry Fee of {0}", text);
                Console.WriteLine("Count : {0}", list.Count);
                foreach (var value in list)
                    Console.WriteLine("{0}, {1}", value.Id, value.StudentFullName);
                Console.WriteLine();
            }
        }

        static void Test10()
        {
            using (var sms = new SMSUnitOfWork())
            {
                //var id = new Guid("6B2FF31C-F3DA-E411-B7D6-FF0411E1DDD9");
                //var guardians = sms.Context.Guardians.Where(m => m.StudentGuardians.Count > 1);
                //var students = sms.GetStudentSections(id, true).Select(m => m.Student);
                //var query = (from sg in guardians.SelectMany(m => m.StudentGuardians)
                //             join s in students on sg.Student.StudentId equals s.StudentId
                //             select sg)
                //            .Include(m => m.Guardian).Include(m => m.Student)
                //            .AsNoTracking().ToList();
                //var male = query.Where(m => m.Guardian.Gender).ToList();
                //var female = query.Where(m => m.Guardian.Gender == false).ToList();

                var id = new Guid("6B2FF31C-F3DA-E411-B7D6-FF0411E1DDD9");
                var guardians = sms.Context.Guardians.Where(m => m.StudentGuardians.Count > 1)
                    .Include(m => m.StudentGuardians.Select(t => t.Student));
                var students = sms.GetStudentSections(id, null).Select(m => m.Student);
                var query = (from g in guardians
                             let sgs = g.StudentGuardians
                             from sg in sgs
                             join s in students on sg.Student.StudentId equals s.StudentId
                             select g)
                            .Include(m => m.StudentGuardians.Select(t => t.Student))
                            .AsNoTracking().ToList();
                var fathers = query.Where(m => m.Gender).Distinct(GuardianComparer.Default).ToList();
                var mothers = query.Where(m => m.Gender == false).Distinct(GuardianComparer.Default).ToList();

                Console.WriteLine("All Total : {0}", query.Count());
                Console.WriteLine("Father Total : {0}", fathers.Count());
                Console.WriteLine("Mother Total : {0}", mothers.Count());
                Console.WriteLine();

                var blist = new List<KeyValues<int, Guardian, Guardian>>();
                var flist = new List<KeyValues<int, Guardian>>();
                var mlist = new List<KeyValues<int, Guardian>>();
                int bint = 0, fint = 0, mint = 0;

                foreach (var father in fathers.OrderBy(m => m.GuardianName))
                {
                    var sids = father.StudentGuardians.Select(m => m.StudentId);
                    bool flag = false;

                    foreach (var mother in mothers)
                    {
                        if (mother.StudentGuardians.Select(m => m.StudentId).SequenceEqual(sids))
                        {
                            blist.Add(new KeyValues<int, Guardian, Guardian>(++bint, father, mother));
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        flist.Add(new KeyValues<int, Guardian>(++fint, father));
                    }
                }

                foreach (var mother in mothers.OrderBy(m => m.GuardianName))
                {
                    var sids = mother.StudentGuardians.Select(m => m.StudentId);
                    bool flag = false;

                    foreach (var father in fathers)
                    {
                        if (father.StudentGuardians.Select(m => m.StudentId).SequenceEqual(sids))
                        {
                            if (blist.FirstOrDefault(m => m.Value1.GuardianId == father.GuardianId
                                && m.Value2.GuardianId == mother.GuardianId) == null)
                            {
                                blist.Add(new KeyValues<int, Guardian, Guardian>(++bint, father, mother));
                            }
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        mlist.Add(new KeyValues<int, Guardian>(++mint, mother));
                    }
                }

                foreach (var item in blist)
                {
                    Console.WriteLine(item.Key);
                    Console.WriteLine("Father : {0}", item.Value1.GuardianName);
                    Console.WriteLine("Mother : {0}", item.Value2.GuardianName);
                    foreach (var student in item.Value1.StudentGuardians.Select(m => m.Student))
                    {
                        Console.WriteLine("Student : {0}, {1}", student.Id, student.StudentFullName);
                    }
                    Console.WriteLine();
                }
                foreach (var item in flist)
                {
                    Console.WriteLine(item.Key);
                    Console.WriteLine("Father : {0}", item.Value1.GuardianName);
                    foreach (var student in item.Value1.StudentGuardians.Select(m => m.Student))
                    {
                        Console.WriteLine("Student : {0}, {1}", student.Id, student.StudentFullName);
                    }
                    Console.WriteLine();
                }
                foreach (var item in mlist)
                {
                    Console.WriteLine(item.Key);
                    Console.WriteLine("Mother : {0}", item.Value1.GuardianName);
                    foreach (var student in item.Value1.StudentGuardians.Select(m => m.Student))
                    {
                        Console.WriteLine("Student : {0}, {1}", student.Id, student.StudentFullName);
                    }
                    Console.WriteLine();
                }
            }
        }

        static void ShuffleData()
        {
            using (SMSContext cox = new SMSContext())
            {
                System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

                //// Address
                //var xa = new Guid[] { new Guid("AB4E725B-C7DA-E411-B7D6-FF0411E1DDD9"), new Guid("AC4E725B-C7DA-E411-B7D6-FF0411E1DDD9") };
                //var list = cox.Addresses.Where(m => xa.Contains(m.AddressId) == false).ToList();
                //int count = list.Count;
                //int half = count / 2;
                //int index1 = 0, index2 = half;

                ////Console.WriteLine("{0}, {1}", count, half);
                ////Console.WriteLine("{0}, {1}", index1, index2);
                //for (index2 = half; index2 < count; index1++, index2++)
                //{
                //    if (index1 < count && index2 < count)
                //    {
                //        var a1 = list[index1];
                //        var a2 = list[index2];
                //        var line1 = a1.Line1;
                //        var township = a1.Township;

                //        a1.Line1 = a2.Line1;
                //        a2.Line1 = line1;
                //        a1.Township = a2.Township;
                //        a2.Township = township;
                //    }
                //}
                ////Console.WriteLine("{0}, {1}", index1, index2);
                //cox.SaveChanges();

                //Random r = new Random();
                //string p = "09421029779";

                //int num = 0;
                //if (int.TryParse(p.Substring(p.Length - 3), out num))
                //{
                //    num += r.Next(1000);
                //    if (num > 999)
                //        num -= 1000;
                //    string numstr = num.ToString().PadLeft(3, '0');
                //    string newp = p.Substring(0, p.Length - 3) + numstr;

                //    Console.WriteLine(p);
                //    Console.WriteLine(numstr);
                //    Console.WriteLine(newp);
                //}

                //// Phone
                //Random r = new Random();
                //var list = cox.StudentPhones.ToList();

                //foreach (var v in list.AsParallel())
                //{
                //    if (v.PhoneNumber.Length < 3)
                //    {
                //        v.PhoneNumber = "097986" + r.Next(100000);
                //    }
                //    else
                //    {
                //        int num = 0;
                //        if (int.TryParse(v.PhoneNumber.Substring(v.PhoneNumber.Length - 3), out num))
                //        {
                //            num += r.Next(1000);
                //            if (num > 999)
                //                num -= 1000;
                //            v.PhoneNumber = v.PhoneNumber.Substring(0, v.PhoneNumber.Length - 3) +
                //                num.ToString().PadLeft(3, '0');
                //        }
                //    }
                //}

                //int count = list.Count;
                //int half = count / 2;
                //int index1 = 0, index2 = half;
                //for (index2 = half; index2 < count; index1++, index2++)
                //{
                //    if (index1 < count && index2 < count)
                //    {
                //        var a1 = list[index1];
                //        var a2 = list[index2];
                //        var pn = a1.PhoneNumber;
                //        var ext = a1.Extension;

                //        a1.PhoneNumber = a2.PhoneNumber;
                //        a2.PhoneNumber = pn;
                //        a1.Extension = a2.Extension;
                //        a2.Extension = ext;
                //    }
                //}
                //cox.SaveChanges();

                //var list = cox.Students.Where(m => m.NIRC != null).ToList();
                //int count = list.Count;
                //int half = count / 2;
                //int index1 = 0, index2 = half;
                //for (index2 = half; index2 < count; index1++, index2++)
                //{
                //    if (index1 < count && index2 < count)
                //    {
                //        var a1 = list[index1];
                //        var a2 = list[index2];
                //        var nirc = a1.NIRC;

                //        a1.NIRC = a2.NIRC;
                //        a2.NIRC = nirc;
                //    }
                //}
                //cox.SaveChanges();

                //var list = cox.Guardians.Where(m => m.Gender == false && m.NIRC != null).ToList();
                //int count = list.Count;
                //int half = count / 2;
                //int index1 = 0, index2 = half;
                //for (index2 = half; index2 < count; index1++, index2++)
                //{
                //    if (index1 < count && index2 < count)
                //    {
                //        var a1 = list[index1];
                //        var a2 = list[index2];
                //        var nirc = a1.NIRC;

                //        a1.NIRC = a2.NIRC;
                //        a2.NIRC = nirc;
                //    }
                //}
                //cox.SaveChanges();

                //Random r = new Random();
                //var list = cox.StudentEmails.Where(m => m.EmailAddress != null).ToList();
                //var gm = "@gmail.com";
                //foreach (var v in list.AsParallel())
                //{
                //    int index = v.EmailAddress.LastIndexOf('@');
                //    if (index != -1)
                //    {
                //        if (v.EmailAddress.Substring(index) == gm)
                //            v.EmailAddress = v.EmailAddress.Insert(index, r.Next(9999).ToString());
                //        else
                //            v.EmailAddress = v.EmailAddress.Substring(0, index) + r.Next(9999).ToString() + gm;
                //    }
                //}
                //cox.SaveChanges();

                //var list = cox.Guardians.Where(m => m.Gender == false).ToList();
                //int count = list.Count;
                //int half = count / 2;
                //int index1 = 0, index2 = half;
                //for (index2 = half; index2 < count; index1++, index2++)
                //{
                //    if (index1 < count && index2 < count)
                //    {
                //        var a1 = list[index1];
                //        var a2 = list[index2];
                //        var title = a1.Title;
                //        var name = a1.GuardianName;
                //        var nation = a1.Nationality;
                //        var nirc = a1.NIRC;
                //        var edu = a1.Education;
                //        var emp = a1.Employment;
                //        var des = a1.Designation;

                //        a1.Title = a2.Title;
                //        a2.Title = title;
                //        a1.GuardianName = a2.GuardianName;
                //        a2.GuardianName = name;
                //        a1.Nationality = a2.Nationality;
                //        a2.Nationality = nation;
                //        a1.NIRC = a2.NIRC;
                //        a2.NIRC = nirc;
                //        a1.Education = a2.Education;
                //        a2.Education = edu;
                //        a1.Employment = a2.Employment;
                //        a2.Employment = emp;
                //        a1.Designation = a2.Designation;
                //        a2.Designation = des;
                //    }
                //}
                //cox.SaveChanges();

                Console.WriteLine("Total query time: {0} ms", sw.Elapsed);
            }
        }

        static void Test14()
        {
            using (var sms = new SMSUnitOfWork())
            {
                var esf = new EntityState[] { EntityState.Added, EntityState.Deleted, EntityState.Modified };

                var s = sms.First<School>();
                var a = new Address { Line1 = "Line1", Country = sms.Countries.GetDefault("MM") };

                s.Address = a;
                Console.WriteLine(sms.GetStringFromChangeTracker(false, null, true, esf));
                Console.WriteLine();

                sms.RefreshToOriginalValues(s);
                //if (s.Address != a)
                //{
                //    var entry = sms.Entry(a);
                //    if (entry.State == EntityState.Added)
                //        entry.State = EntityState.Detached;
                //}
                sms.AddedToDetached(a);
                Console.WriteLine(sms.GetStringFromChangeTracker(false, null, true, esf));
                Console.WriteLine();

                //Console.WriteLine("Save Changes : {0}", sms.SaveChanges());
            }
        }

        static void Test15()
        {
            using (var sms = new SMSUnitOfWork())
            {
                var esf = new EntityState[] { EntityState.Added, EntityState.Deleted, EntityState.Modified };

                var s = sms.First<School>();
                s.SchoolEmails.Add(new SchoolEmail { EmailAddress = "school1.2@gmail.com", EmailTypeId = Guid.Parse("E8592284-C3DA-E411-B7D6-FF0411E1DDD9") });

                Console.WriteLine(s.SchoolEmails.Count);
                Console.WriteLine(sms.GetStringFromChangeTracker(false, null, true, esf));
                Console.WriteLine();

                sms.RefreshRangeToOriginalValues(s.SchoolEmails, true);

                Console.WriteLine(s.SchoolEmails.Count);
                Console.WriteLine(sms.GetStringFromChangeTracker(false, null, true, esf));
                Console.WriteLine();

                var t = typeof(SchoolEmail);
                var list = sms.Context.ChangeTracker.Entries().Where(m => m.State == EntityState.Added
                    && ObjectContext.GetObjectType(m.Entity.GetType()) == t);

                foreach (var l in list)
                    Console.WriteLine("{0}, {1}, {2}", l.Entity, ((SchoolEmail)l.Entity).School, l.State);
                Console.WriteLine();

                Console.WriteLine("Save Changes : {0}", sms.SaveChanges());
            }
        }

        static void Test16()
        {
            using (var sms = new SMSUnitOfWork())
            {
                string un = "administrator";
                string pw = "sms@2015";
                var entity = sms.GetUserByUserName(un, asNoTracking: true);

                if (entity == null)
                    Console.WriteLine("<null>");
                else
                {
                    bool flag = CodeflexSystems.Library.Security.Crypto.VerifyHashedRfc2898(entity.UserMembership.Password, pw);

                    if (flag)
                        Console.WriteLine("valid password.");
                    else
                        Console.WriteLine("invalid password.");
                }
            }
        }

        static void Test17()
        {
            using (var sms = new SMSUnitOfWork())
            {
                string em = "administrator@SMS.com";
                //string name = sms.GetUserNameByEmail(em);
                var predicate = ExpressionHelper.CreatePredicate<User>("EmailAddress", em, StringMatchType.Equal);
                var entity = sms.FirstOrDefault(predicate);
                string name = entity == null ? null : entity.UserName;

                if (name == null)
                    Console.WriteLine("<null>");
                else
                    Console.WriteLine(name);
            }
        }

        static void Test19()
        {
            using (var sms = new SMSUnitOfWork())
            {
                var entity = sms.SelectAll<Section>()
                    .Include(m => m.Class)
                    .FirstOrDefault();

                Console.WriteLine(entity.SectionName);

                //var entity = sms.SelectAll<Class>()
                //    .Include(m => m.Course.AcademicYear.School)
                //    .FirstOrDefault();

                //Console.WriteLine(entity.ClassName);
            }
        }

        static void Test20()
        {
            using (var sms = new SMSUnitOfWork())
            {
                //var filter = sms.Where<Student>(m => m.StudentName == "Hnin May Aung");
                //var q1 = from t in filter.SelectMany(m => m.StudentSections)
                //         where t.StudentDiscount != null
                //         select t.Student;

                //Console.WriteLine(q1.AsNoTracking().ToString());
                //Console.WriteLine();
                //Console.WriteLine();
                //Console.WriteLine(q1.Distinct().AsNoTracking().ToString());

                var filter = sms.Where<Student>(m => m.StudentName == "Hnin May Aung");
                var q1 = (from t in filter.SelectMany(m => m.StudentSections)
                          where t.StudentDiscount != null
                          select t.StudentId).Distinct();
                var q3 = sms.SelectAll<Student>().Join(q1, m => m.StudentId, u => u, (m, u) => m);

                Console.WriteLine(q3.Ordered().AsNoTracking().ToString());
            }
        }

        static void Test21()
        {
            using (var cox = new SMSContext())
            {
                Guid id = Guid.Parse("57D2D34F-FDDA-E411-B7D6-FF0411E1DDD9");
                var query = cox.StudentSections.Where(m => m.RegisteredDate.HasValue &&
                    m.RegisteredDate.Value.Year == 2016 && m.RegisteredDate.Value.Month > 5)
                    .Include(m => m.Section.Class);

                //Console.WriteLine(query.Where(m => m.Section.Class.CourseId == id).Count());
                foreach (var m in query.Where(m => m.Section.Class.CourseId == id))
                    m.RegisteredDate = new DateTime(2016, 5, 26);
                cox.SaveChanges();
            }
        }

        static void Print(ISMSUnitOfWork sms, User u)
        {
            var ue = sms.Entry(u);
            var me = sms.Entry(u.UserMembership);

            Console.WriteLine("User : {0}", ue.State);
            Console.WriteLine("UserMembership : {0}", me.State);
            Console.WriteLine();
        }

        static void Print(IEnumerable<RelationshipEndMember> array)
        {
            foreach (var item in array)
                Console.WriteLine("{0} - {1}", item.ToString(), item.RelationshipMultiplicity);
        }

        static IEnumerable<EntityReference> EnumerateSaveReferences(RelationshipManager manager)
        {
            return manager.GetAllRelatedEnds().OfType<EntityReference>()
                    .Where(er => er.RelationshipSet.ElementType.RelationshipEndMembers[er.SourceRoleName].RelationshipMultiplicity != RelationshipMultiplicity.One &&
                        !((AssociationSet)er.RelationshipSet).ElementType.IsForeignKey);
        }

        static void PrintChangeTracker(ISMSUnitOfWork sms, bool include = true, params Type[] filterTypes)
        {
            Console.WriteLine("Total entries count : {0}", sms.Context.ChangeTracker.Entries().Count());
            if (filterTypes.Length == 0)
            {
                foreach (var entity in sms.Context.ChangeTracker.Entries())
                {
                    var type = ObjectContext.GetObjectType(entity.Entity.GetType());
                    Console.WriteLine("{0}, {1}, {2}", type, entity.State, entity.Entity.ToString());
                }
            }
            else
            {
                foreach (var entity in sms.Context.ChangeTracker.Entries())
                {
                    var type = ObjectContext.GetObjectType(entity.Entity.GetType());
                    //System.Data.Entity.DynamicProxies.

                    if (include)
                    {
                        if (filterTypes.Contains(type))
                            Console.WriteLine("{0}, {1}, {2}", type, entity.State, entity.Entity.ToString());
                    }
                    else
                    {
                        if (!filterTypes.Contains(type))
                            Console.WriteLine("{0}, {1}, {2}", type, entity.State, entity.Entity.ToString());
                    }
                }
            }
            Console.WriteLine();
        }

        static void PrintChangeTracker(ISMSUnitOfWork sms, bool includeType, Type[] filterTypes, bool includeState, EntityState[] filterStates)
        {
            var entities = sms.Context.ChangeTracker.Entries();
            Console.WriteLine("Total entries count : {0}", entities.Count());

            if (filterTypes != null && filterTypes.Length > 0)
            {
                if (includeType)
                    entities = entities.Where(m => filterTypes.Contains(ObjectContext.GetObjectType(m.Entity.GetType())));
                else
                    entities = entities.Where(m => !filterTypes.Contains(ObjectContext.GetObjectType(m.Entity.GetType())));
            }

            if (filterStates != null && filterStates.Length > 0)
            {
                if (includeState)
                    entities = entities.Where(m => filterStates.Contains(m.State));
                else
                    entities = entities.Where(m => !filterStates.Contains(m.State));
            }

            foreach (var entity in entities)
                Console.WriteLine("{0}, {1}, {2}", ObjectContext.GetObjectType(entity.Entity.GetType()),
                    entity.State, entity.Entity.ToString());
            Console.WriteLine();
        }

        private static void ShowChangeTracker(ISMSUnitOfWork sms)
        {
            var entities = sms.Context.ChangeTracker.Entries();
            Console.WriteLine("Total entries count : {0}", entities.Count());

            foreach (var entity in entities)
                Console.WriteLine("{0}, {1}, {2}", ObjectContext.GetObjectType(entity.Entity.GetType()),
                    entity.State, entity.Entity.ToString());
            Console.WriteLine();
        }

        private static void WriteEntry(ISMSUnitOfWork uow, DbEntityEntry entry, DbEntityEntry parent, bool first)
        {
            if (!first && entry.Entity == parent.Entity)
            {
                Console.WriteLine("break...");
                Console.WriteLine();
                return;
            }

            if (entry.State == EntityState.Modified)
            {
                Console.WriteLine("{0}", ObjectContext.GetObjectType(entry.Entity.GetType()));
                PrintEntry(uow, entry);
                Console.WriteLine();
                //PrintPropertyValues(entry.CurrentValues, entry.CurrentValues.PropertyNames);
                entry.Reload();
            }
            //else if (entry.State == EntityState.Added)
            //{
            //    entry.State = EntityState.Detached;
            //    return;
            //}
            //else
            //{
            //    Console.WriteLine("{0}, {1}", ObjectContext.GetObjectType(entry.Entity.GetType()), entry.State);
            //    Console.WriteLine();
            //    //return;
            //}

            ObjectStateEntry ose;
            if (!uow.ObjectContext.ObjectStateManager
                .TryGetObjectStateEntry(entry.Entity, out ose))
                return;

            var entityType = ose.EntitySet.ElementType as EntityType;
            if (entityType == null)
                return;
            //if (ose.IsRelationship)
            //    Console.WriteLine("IsRelationship - {0}", ose.IsRelationship);

            foreach (var property in entityType.NavigationProperties)
            {
                var type = property.TypeUsage.EdmType;

                if (type is CollectionType)
                {
                    var collection = entry.Collection(property.Name);
                    if (collection.IsLoaded)
                    {
                        //Console.WriteLine("{0}, {1}, {2}", collection.Name, collection.IsLoaded,
                        //    collection.CurrentValue);
                        foreach (var item in collection.CurrentValue as ICollection)
                        {
                            var centry = uow.Entry(item);
                            //if (centry.State == EntityState.Modified)
                            {
                                WriteEntry(uow, centry, parent, false);
                            }
                        }
                    }
                }
                else if (type is StructuralType)
                {
                    var value = entry.Reference(property.Name);
                    if (value.IsLoaded)
                    {
                        //Console.WriteLine("{0}, {1}, {2}", value.Name, value.IsLoaded, 
                        //    value.CurrentValue);
                        var rentry = uow.Entry(value.CurrentValue);
                        //if (rentry.State == EntityState.Modified)
                        {
                            WriteEntry(uow, rentry, parent, false);
                        }
                    }
                }
            }
        }

        private static void PrintEntry(ISMSUnitOfWork uow, DbEntityEntry entry)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    Console.WriteLine("Adding a {0}", entry.Entity.GetType());
                    PrintPropertyValues(entry.CurrentValues,
                        entry.CurrentValues.PropertyNames);
                    break;
                case EntityState.Deleted:
                    Console.WriteLine("Deleting a {0}", entry.Entity.GetType().BaseType);
                    PrintPropertyValues(entry.OriginalValues,
                        GetKeyPropertyNames(uow, entry.Entity));
                    break;
                case EntityState.Modified:
                    Console.WriteLine("Modifying a {0}", entry.Entity.GetType().BaseType);
                    var modifiedPropertyNames = from n in entry.CurrentValues.PropertyNames
                                                where entry.Property(n).IsModified
                                                select n;
                    PrintPropertyValues(entry.CurrentValues,
                        GetKeyPropertyNames(uow, entry.Entity)
                        .Concat(modifiedPropertyNames));
                    break;
            }
        }

        private static void PrintPropertyValues(DbPropertyValues values,
            IEnumerable<string> propertiesToPrint, int indent = 1)
        {
            foreach (var propertyName in propertiesToPrint)
            {
                var value = values[propertyName];
                if (value is DbPropertyValues)
                {
                    Console.WriteLine("{0}- Complex Property: {1}",
                        string.Empty.PadLeft(indent), propertyName);
                    var complexPropertyValues = (DbPropertyValues)value;
                    PrintPropertyValues(complexPropertyValues,
                        complexPropertyValues.PropertyNames, indent + 1);
                }
                else
                {
                    Console.WriteLine("{0}- {1}: {2}",
                        string.Empty.PadLeft(indent), propertyName, values[propertyName]);
                }
            }
        }

        private static IEnumerable<string> GetKeyPropertyNames(ISMSUnitOfWork uow, object entity)
        {
            var objectContext = uow.ObjectContext;
            return objectContext
                .ObjectStateManager
                .GetObjectStateEntry(entity)
                .EntityKey
                .EntityKeyValues
                .Select(k => k.Key);
        }

        static void ShowObjectStateEntry<T>(ISMSUnitOfWork uow, T entity)
        {
            if (entity == null)
                return;

            ObjectStateEntry ose;
            if (!uow.ObjectContext.ObjectStateManager.TryGetObjectStateEntry(entity,
                out ose))
                return;

            int index = 1;
            if (ose.Entity is SchoolPhone)
                index = 2;
            Console.WriteLine(ose.EntitySet);
            if (ose.State == EntityState.Added || ose.State == EntityState.Detached)
                Console.WriteLine("{0}, c - {1}", ose.State, ose.CurrentValues[index]);
            else
                Console.WriteLine("{0}, c - {1}, o - {2}", ose.State, ose.CurrentValues[index],
                    ose.OriginalValues[index]);
        }

        private static void BrowseTypesExample(MetadataWorkspace workspace)
        {
            // Get a collection of the GlobalItems. 
            // An GlobalItem class is the base class for 
            // the conceptual model types and entity containers.
            ReadOnlyCollection<GlobalItem> items =
                 workspace.GetItems<GlobalItem>(DataSpace.CSpace);

            // Iterate through the collection to get each item.
            foreach (GlobalItem item in items)
            {
                Console.WriteLine(
                    "Item BuiltInTypeKind: {0}, Type: {1} ",
                    item.BuiltInTypeKind, item.GetType().FullName);

                EntityContainer entityContainer = item as EntityContainer;
                if (entityContainer != null)
                {
                    Console.WriteLine(
                         "EntityContainer Name: {0}",
                         entityContainer.Name);
                    continue;
                }

                EntityType entityType = item as EntityType;
                if (entityType != null)
                {
                    Console.WriteLine(
                        "EntityType Name: {0}, Namespace: {1}",
                        entityType.Name, entityType.NamespaceName);
                    continue;
                }

                AssociationType associationType = item as AssociationType;
                if (associationType != null)
                {
                    Console.WriteLine(
                       "AssociationType Name: {0}, Namespace: {1}",
                       associationType.Name, associationType.NamespaceName);
                    continue;
                }

                PrimitiveType primType = item as PrimitiveType;
                if (primType != null)
                {
                    Console.WriteLine(
                        "PrimitiveType Name: {0}, Namespace: {1}",
                        primType.Name, primType.NamespaceName);
                    continue;
                }

                EdmFunction function = item as EdmFunction;
                if (function != null)
                {
                    Console.WriteLine(
                      "EdmFunction Name: {0}, Namespace: {1}",
                      function.Name, function.NamespaceName);
                    continue;
                }
            }
        }
    }

    //public static class CollectionExtension
    //{
    //    public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Expression<Func<TSource, bool>> predicate)
    //    {
    //        return source.Where(predicate.Compile());
    //    }

    //    public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Expression<Func<TSource, int, bool>> predicate)
    //    {
    //        return source.Where(predicate.Compile());
    //    }
    //}
}
