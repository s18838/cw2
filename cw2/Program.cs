using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace cw2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var path = @"C:\Users\s18838\Desktop\dane.csv";

            var lines = File.ReadLines(path);

            var students = new HashSet<Student>(new StudentComparer());
            var studies = new HashSet<Study>();

            foreach (var line in lines)
            {

                try
                {

                    var array = line.Split(",");

                    if (array.Length < 9) throw new NotRightData(); ;

                    foreach (var value in array)
                    {
                        if (value.Length == 0) throw new NotFullData();
                    }

                    var study = new Study { Name = array[2] };

                    bool b = true;
                    foreach (var s in studies)
                    {
                        if (s.Equals(study))
                        {
                            s.CountOfStudents++;
                            b = false;
                        }
                    }

                    if (b)
                    {
                        studies.Add(new Study { Name = array[2], CountOfStudents = 1 });
                    }

                    var student = new Student
                    {
                        Name = array[0],
                        Surname = array[1],
                        Lecture = new Lecture { Name = array[2], Type = array[3] },
                        Index = int.Parse(array[4]),
                        Birthday = DateTime.Parse(array[5]),
                        Email = array[6],
                        MothersName = array[7],
                        FathersName = array[8]
                    };

                    if (students.Contains(student)) throw new DuplicateData();

                    students.Add(student);

                }
                catch (Exception e)
                {

                }

            }

            FileStream writer = new FileStream(@"data.xml", FileMode.Create);


            XmlSerializer serializer = new XmlSerializer(typeof(University));

            var listStudents = new List<Student>();

            foreach (var student in students)
            {
                listStudents.Add(student);
            }

            var listStudies = new List<Study>();

            foreach (var study in studies)
            {
                listStudies.Add(study);
            }

            var university = new University { Author = "Taras Kulyavtes",
                CreatedAt = DateTime.Now,
                Students = listStudents,
                ActiveStudies = listStudies
            };

            serializer.Serialize(writer, university);

        }
    }

    class NotFullData : Exception { }
    class NotRightData : Exception { }
    class DuplicateData : Exception { }

    class StudentComparer : IEqualityComparer<Student>
    {
        public bool Equals(Student x, Student y)
        {
            if (x.Index != y.Index) return false;
            if (!x.Name.Equals(y.Name)) return false;
            if (!x.Surname.Equals(y.Surname)) return false;
            return true;
        }

        public int GetHashCode(Student obj)
        {
            return obj.Index;
        }
    }

    public class Student {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string MothersName { get; set; }
        public string FathersName { get; set; }
        public int Index { get; set; }
        public DateTime Birthday { get; set; }
        public Lecture Lecture { get; set; }
    }

    public class Lecture
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class Study {
        public string Name { get; set; }
        public int CountOfStudents { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!(obj is Study))
            {
                return false;
            }
            return this.Name == ((Study)obj).Name;
        }

        public int GetHashCode(Student obj)
        {
            return obj.Name.GetHashCode();
        }
    }

    public class University {
        public DateTime CreatedAt { get; set; }
        public string Author { get; set; }
        public List<Student> Students { get; set; }
        public List<Study> ActiveStudies { get; set; }
    }
}
