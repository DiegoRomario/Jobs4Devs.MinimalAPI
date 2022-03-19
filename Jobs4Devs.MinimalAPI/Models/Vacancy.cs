namespace Jobs4Devs.MinimalAPI.Models
{
    public class Vacancy
    {
        public Vacancy()
        {

        }
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Company { get; set; }
        public bool IsOpen { get; set; }
        public double MinSalary { get; set; }
        public double MaxSalary { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
