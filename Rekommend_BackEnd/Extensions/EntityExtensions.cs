using Rekommend_BackEnd.Entities;
using Rekommend_BackEnd.Models;

namespace Rekommend_BackEnd.Extensions
{
    public static class EntityExtensions
    {
        public static TechJobOpeningDto ToDto(this TechJobOpening source)
        {
            return new TechJobOpeningDto()
            {
                Id = source.Id,
                CreationDate = source.CreationDate,
                ClosingDate = source.ClosingDate,
                StartingDate = source.StartingDate,
                Title = source.Title,
                CompanyId = source.Recruiter.CompanyId,
                CompanyName = source.Recruiter.Company.Name,
                CompanyCategory = source.Recruiter.Company.Category.ToString(),
                RecruiterId = source.RecruiterId,
                RecruiterFirstName = source.Recruiter.FirstName,
                RecruiterLastName = source.Recruiter.LastName,
                RecruiterPosition = source.Recruiter.Position.ToString(),
                JobTechLanguage = source.JobTechLanguage.ToString(),
                JobPosition = source.JobPosition.ToString(),
                Seniority = source.Seniority.ToString(),
                ContractType = source.ContractType.ToString(),
                RemoteWorkAccepted = source.RemoteWorkAccepted,
                MissionDescription = source.MissionDescription,
                City = source.City.ToString(),
                PostCode = source.PostCode,
                Country = source.Country.ToString(),
                Reward1 = source.Reward1,
                Reward2 = source.Reward2,
                Reward3 = source.Reward3,
                LikesNb = source.LikesNb,
                RekommendationsNb = source.RekommendationsNb,
                ViewsNb = source.ViewsNb,
                MinimumSalary = source.MinimumSalary,
                MaximumSalary = source.MaximumSalary,
                Status = source.Status,
                PictureFileName = source.PictureFileName,
                RseDescription = source.RseDescription
            };
        }

        public static TechJobOpeningForUpdateDto ToUpdateDto(this TechJobOpening source)
        {
            return new TechJobOpeningForUpdateDto()
            {
                StartingDate = source.StartingDate,
                Title = source.Title,
                JobTechLanguage = source.JobTechLanguage.ToString(),
                JobPosition = source.JobPosition.ToString(),
                Seniority = source.Seniority.ToString(),
                ContractType = source.ContractType.ToString(),
                RemoteWorkAccepted = source.RemoteWorkAccepted,
                MissionDescription = source.MissionDescription,
                City = source.City.ToString(),
                PostCode = source.PostCode,
                Country = source.Country.ToString(),
                MinimumSalary = source.MinimumSalary,
                MaximumSalary = source.MaximumSalary,
                Reward1 = source.Reward1,
                Reward2 = source.Reward2,
                Reward3 = source.Reward3,
                PictureFileName = source.PictureFileName,
                RseDescription = source.RseDescription
            };
        }

        public static TechJobOpening ToEntity(this TechJobOpeningForCreationDto source)
        {
            return new TechJobOpening()
            {
                StartingDate = source.StartingDate,
                Title = source.Title,
                JobTechLanguage = source.JobTechLanguage.ToJobTechLanguage(),
                JobPosition = source.JobPosition.ToPosition(),
                Seniority = source.Seniority.ToSeniority(),
                ContractType = source.ContractType.ToContractType(),
                RemoteWorkAccepted = source.RemoteWorkAccepted,
                MissionDescription = source.MissionDescription,
                City = source.City,
                PostCode = source.PostCode,
                Country = source.Country.ToCountry(),
                MinimumSalary = source.MinimumSalary,
                MaximumSalary = source.MaximumSalary
            };
        }

        public static TechJobOpening ToEntity(this TechJobOpeningForUpdateDto source)
        {
            return new TechJobOpening()
            {
                StartingDate = source.StartingDate,
                Title = source.Title,
                JobTechLanguage = source.JobTechLanguage.ToJobTechLanguage(),
                JobPosition = source.JobPosition.ToPosition(),
                Seniority = source.Seniority.ToSeniority(),
                ContractType = source.ContractType.ToContractType(),
                RemoteWorkAccepted = source.RemoteWorkAccepted,
                MissionDescription = source.MissionDescription,
                City = source.City,
                PostCode = source.PostCode,
                Country = source.Country.ToCountry(),
                MinimumSalary = source.MinimumSalary,
                MaximumSalary = source.MaximumSalary,
                Reward1 = source.Reward1,
                Reward2 = source.Reward2,
                Reward3 = source.Reward3,
                PictureFileName = source.PictureFileName,
                RseDescription = source.RseDescription
            };
        }

        public static RecruiterDto ToDto(this Recruiter source)
        {
            return new RecruiterDto()
            {
                Id = source.Id,
                RegistrationDate = source.RegistrationDate,
                FirstName = source.FirstName,
                LastName = source.LastName,
                CompanyId = source.CompanyId,
                Position = source.Position.ToString(),
                Age = source.DateOfBirth.GetCurrentAge(),
                Email = source.Email,
                Gender = source.Gender.ToString()
            };
        }

        public static Recruiter ToEntity(this RecruiterForCreationDto source)
        {
            return new Recruiter()
            {
                FirstName = source.FirstName,
                LastName = source.LastName,
                Position = source.Position.ToRecruiterPosition(),
                DateOfBirth = source.DateOfBirth,
                Email = source.Email,
                Gender = source.Gender.ToGender()
            };
        }

        public static Recruiter ToEntity(this RecruiterForUpdateDto source)
        {
            return new Recruiter()
            {
                FirstName = source.FirstName,
                LastName = source.LastName,
                Position = source.Position.ToRecruiterPosition(),
                DateOfBirth = source.DateOfBirth,
                Email = source.Email,
                Gender = source.Gender.ToGender()
            };
        }

        public static RecruiterForUpdateDto ToUpdateDto(this Recruiter source)
        {
            return new RecruiterForUpdateDto()
            {
                FirstName = source.FirstName,
                LastName = source.LastName,
                Position = source.Position.ToString(),
                DateOfBirth = source.DateOfBirth,
                Email = source.Email,
                Gender = source.Gender.ToString()
            };
        }

        public static CompanyDto ToDto(this Company source)
        {
            return new CompanyDto()
            {
                Id = source.Id,
                RegistrationDate = source.RegistrationDate,
                Name = source.Name,
                HqCity = source.HqCity,
                HqCountry = source.HqCountry,
                PostCode = source.PostCode,
                CompanyDescription = source.CompanyDescription,
                Category = source.Category.ToString(),
                LogoFileName = source.LogoFileName,
                Website = source.Website,
                EmployerBrandWebsite = source.EmployerBrandWebsite
            };
        }

        public static Company ToEntity(this CompanyForCreationDto source)
        {
            return new Company()
            {
                Name = source.Name,
                HqCity = source.HqCity,
                HqCountry = source.HqCountry,
                PostCode = source.PostCode,
                CompanyDescription = source.CompanyDescription,
                Category = source.Category.ToCompanyCategory(),
                LogoFileName = source.LogoFileName,
                Website = source.Website,
                EmployerBrandWebsite = source.EmployerBrandWebsite
            };
        }

        public static Company ToEntity(this CompanyForUpdateDto source)
        {
            return new Company()
            {
                Name = source.Name,
                HqCity = source.HqCity,
                HqCountry = source.HqCountry,
                PostCode = source.PostCode,
                CompanyDescription = source.CompanyDescription,
                Category = source.Category.ToCompanyCategory(),
                LogoFileName = source.LogoFileName,
                Website = source.Website,
                EmployerBrandWebsite = source.EmployerBrandWebsite
            };
        }

        public static CompanyForUpdateDto ToUpdateDto(this Company source)
        {
            return new CompanyForUpdateDto()
            {
                Name = source.Name,
                HqCity = source.HqCity,
                HqCountry = source.HqCountry,
                PostCode = source.PostCode,
                CompanyDescription = source.CompanyDescription,
                Category = source.Category.ToString(),
                LogoFileName = source.LogoFileName,
                Website = source.Website,
                EmployerBrandWebsite = source.EmployerBrandWebsite
            };
        }

        public static RekommendationDto ToDto(this Rekommendation source)
        {
            return new RekommendationDto()
            {
                Id = source.Id,
                CreationDate = source.CreationDate,
                StatusChangeDate = source.StatusChangeDate,
                RekommenderId = source.RekommenderId,
                RekommenderFirstName = source.Rekommender.FirstName,
                RekommenderLastName = source.Rekommender.LastName,
                RekommenderPosition = source.Rekommender.Position.ToString(),
                RekommenderSeniority = source.Rekommender.Seniority.ToString(),
                RekommenderCompany = source.Rekommender.Company,
                RekommenderCity = source.Rekommender.City,
                RekommenderPostCode = source.Rekommender.PostCode,
                RekommenderEmail = source.Rekommender.Email,
                TechJobOpeningId = source.TechJobOpeningId,
                TechJobOpeningTitle = source.TechJobOpening.Title,
                FirstName = source.FirstName,
                LastName = source.LastName,
                Position = source.Position.ToString(),
                Seniority = source.Seniority.ToString(),
                Company = source.Company,
                Email = source.Email,
                Comment = source.Comment,
                Status = source.Status.ToString(),
                HasAlreadyWorkedWithRekommender = source.HasAlreadyWorkedWithRekommender
            };
        }

        public static Rekommendation ToEntity(this RekommendationForCreationDto source)
        {
            return new Rekommendation()
            {
                TechJobOpeningId = source.TechJobOpeningId,
                FirstName = source.FirstName,
                LastName = source.LastName,
                Position = source.Position.ToPosition(),
                Seniority = source.Seniority.ToSeniority(),
                Company = source.Company,
                Email = source.Email,
                Comment = source.Comment,
                HasAlreadyWorkedWithRekommender = source.HasAlreadyWorkedWithRekommender
            };
        }

        public static Rekommendation ToEntity(this RekommendationForUpdateDto source)
        {
            return new Rekommendation()
            {
                FirstName = source.FirstName,
                LastName = source.LastName,
                Position = source.Position.ToPosition(),
                Seniority = source.Seniority.ToSeniority(),
                Comment = source.Comment,
                Company = source.Company,
                Email = source.Email,
                Grade = source.Grade,
                Status = source.RekommendationStatus.ToRekommendationStatus()
            };
        }

        public static RekommendationForUpdateDto ToUpdateDto(this Rekommendation source)
        {
            return new RekommendationForUpdateDto()
            {
                FirstName = source.FirstName,
                LastName = source.LastName,
                Position = source.Position.ToString(),
                Seniority = source.Seniority.ToString(),
                Comment = source.Comment,
                RekommendationStatus = source.Status.ToString(),
                Company = source.Company,
                Email = source.Email,
                Grade = source.Grade
            };
        }

        public static RekommenderDto ToDto(this Rekommender source)
        {
            return new RekommenderDto()
            {
                Id = source.Id,
                Age = source.DateOfBirth.GetCurrentAge(),
                RegistrationDate = source.RegistrationDate,
                FirstName = source.FirstName,
                LastName = source.LastName,
                Position = source.Position.ToString(),
                Seniority = source.Seniority.ToString(),
                Company = source.Company,
                City = source.City,
                PostCode = source.PostCode,
                Email = source.Email,
                XpRekommend = source.XpRekommend,
                RekommendationsAvgGrade = source.RekommendationsAvgGrade,
                Level = GetRekommenderLevel(source.XpRekommend)
            };
        }

        public static Rekommender ToEntity(this RekommenderForCreationDto source)
        {
            return new Rekommender()
            {
                DateOfBirth = source.DateOfBirth,
                FirstName = source.FirstName,
                LastName = source.LastName,
                Position = source.Position.ToPosition(),
                Seniority = source.Seniority.ToSeniority(),
                Company = source.Company,
                City = source.City,
                PostCode = source.PostCode,
                Email = source.Email
            };
        }

        public static Rekommender ToEntity(this RekommenderForUpdateDto source)
        {
            return new Rekommender()
            {
                FirstName = source.FirstName,
                LastName = source.LastName,
                Position = source.Position.ToPosition(),
                Seniority = source.Seniority.ToSeniority(),
                Company = source.Company,
                City = source.City,
                PostCode = source.PostCode,
                Email = source.Email
            };
        }

        public static RekommenderForUpdateDto ToUpdateDto(this Rekommender source)
        {
            return new RekommenderForUpdateDto()
            {
                FirstName = source.FirstName,
                LastName = source.LastName,
                Position = source.Position.ToString(),
                Seniority = source.Seniority.ToString(),
                City = source.City,
                PostCode = source.PostCode,
                Company = source.Company,
                Email = source.Email
            };
        }

        private static string GetRekommenderLevel(int xpRekommend)
        {
            if (xpRekommend < 50)
            {
                return "Apprentice";
            }
            else if (xpRekommend < 200)
            {
                return "Craftsman";
            }
            else if (xpRekommend < 500)
            {
                return "Master";
            }
            else if (xpRekommend < 1000)
            {
                return "Giant";
            }
            else
            {
                return "Hero";
            }
        }
    }
}
