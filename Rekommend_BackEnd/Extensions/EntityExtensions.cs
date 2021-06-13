using Rekommend_BackEnd.Entities;
using Rekommend_BackEnd.Models;
using System.Collections.Generic;

namespace Rekommend_BackEnd.Extensions
{
    public static class EntityExtensions
    {
        public static TechJobOpeningDto ToDto(this TechJobOpening source)
        {
            return new TechJobOpeningDto()
            {
                Id = source.Id,
                CreatedOn = source.CreatedOn,
                ClosingDate = source.ClosingDate,
                StartingDate = source.StartingDate,
                Title = source.Title,
                CompanyId = source.CompanyId,
                CompanyName = source.Company.Name,
                CompanyCategory = source.Company.Category.ToString(),
                CreatedById = source.CreatedBy,
                RecruiterFirstName = source.CreatedByFirstName,
                RecruiterLastName = source.CreatedByLastName,
                RecruiterPosition = source.CreatedByPosiion.ToString(),
                JobTechLanguage = source.JobTechLanguage.ToString(),
                JobPosition = source.JobPosition.ToString(),
                Seniority = source.Seniority.ToString(),
                ContractType = source.ContractType.ToString(),
                RemoteWorkAccepted = source.RemoteWorkAccepted,
                MissionDescription = source.MissionDescription,
                City = source.City.ToString(),
                PostCode = source.PostCode,
                Country = source.Country.ToString(),
                Reward = source.Reward,
                BonusReward = source.BonusReward,
                LikesNb = source.LikesNb,
                RekommendationsNb = source.RekommendationsNb,
                ViewsNb = source.ViewsNb,
                MinimumSalary = source.MinimumSalary,
                MaximumSalary = source.MaximumSalary,
                Status = source.Status,
                PictureFileName = source.PictureFileName,
                RseDescription = source.RseDescription,
                Rekommendations = ToRekommendationDtoCollection(source.Rekommendations)
            };
        }

        public static ICollection<RekommendationDto> ToRekommendationDtoCollection(ICollection<Rekommendation> rekommendations)
        {
            List<RekommendationDto> rekommendationDtoList = new List<RekommendationDto>();

            foreach (var reko in rekommendations)
            {
                rekommendationDtoList.Add(
                    new RekommendationDto
                    {
                        Id = reko.Id,
                        CreatedOn = reko.CreatedOn,
                        RekommenderId = reko.AppUser.Id,
                        RekommenderFirstName = reko.AppUser.FirstName,
                        RekommenderLastName = reko.AppUser.LastName,
                        RekommenderProfile = reko.AppUser.Profile.ToString(),
                        RekommenderSeniority = reko.AppUser.Seniority.ToString(),
                        RekommenderCompany = reko.AppUser.Company,
                        RekommenderEmail = reko.AppUser.Email,
                        RekommenderCity = reko.AppUser.City,
                        TechJobOpeningId = reko.TechJobOpeningId,
                        TechJobOpeningTitle = reko.TechJobOpening.Title,
                        FirstName = reko.FirstName,
                        LastName = reko.LastName,
                        Position = reko.Position.ToString(),
                        Seniority = reko.Seniority.ToString(),
                        Company = reko.Company,
                        Email = reko.Email,
                        Comment = reko.Comment,
                        Status = reko.Status.ToString(),
                        HasAlreadyWorkedWithRekommender = reko.HasAlreadyWorkedWithRekommender
                    });
            }
            return rekommendationDtoList;
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
                Reward = source.Reward,
                BonusReward = source.BonusReward,
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
                Reward = source.Reward,
                BonusReward = source.BonusReward,
                PictureFileName = source.PictureFileName,
                RseDescription = source.RseDescription
            };
        }

        public static ExtendedUserDto ToDto(this ExtendedUser source)
        {
            return new ExtendedUserDto()
            {
                Id = source.Id,
                CompanyId = source.CompanyId,
                Position = source.Position.ToString(),
                Age = source.DateOfBirth.GetCurrentAge()
            };
        }

        public static ExtendedUser ToEntity(this ExtendedUserForCreationDto source)
        {
            return new ExtendedUser()
            {
                Position = source.Position.ToRecruiterPosition(),
                DateOfBirth = source.DateOfBirth
            };
        }

        public static ExtendedUser ToEntity(this ExtendedUserForUpdateDto source)
        {
            return new ExtendedUser()
            {
                Position = source.Position.ToRecruiterPosition(),
                DateOfBirth = source.DateOfBirth
            };
        }

        public static ExtendedUserForUpdateDto ToUpdateDto(this ExtendedUser source)
        {
            return new ExtendedUserForUpdateDto()
            {
                Position = source.Position.ToString(),
                DateOfBirth = source.DateOfBirth
            };
        }

        public static CompanyDto ToDto(this Company source)
        {
            return new CompanyDto()
            {
                Id = source.Id,
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
                CreatedOn = source.CreatedOn,
                StatusChangeDate = source.UpdatedOn,
                //RekommenderId = source.RekommenderId,
                //RekommenderFirstName = source.Rekommender.FirstName,
                //RekommenderLastName = source.Rekommender.LastName,
                //RekommenderPosition = source.Rekommender.Position.ToString(),
                //RekommenderSeniority = source.Rekommender.Seniority.ToString(),
                //RekommenderCompany = source.Rekommender.Company,
                //RekommenderCity = source.Rekommender.City,
                //RekommenderPostCode = source.Rekommender.PostCode,
                //RekommenderEmail = source.Rekommender.Email,
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

        //private static string GetUserLevel(int xpRekommend)
        //{
        //    if (xpRekommend < 50)
        //    {
        //        return "Apprentice";
        //    }
        //    else if (xpRekommend < 200)
        //    {
        //        return "Craftsman";
        //    }
        //    else if (xpRekommend < 500)
        //    {
        //        return "Master";
        //    }
        //    else if (xpRekommend < 1000)
        //    {
        //        return "Giant";
        //    }
        //    else
        //    {
        //        return "Hero";
        //    }
        //}
    }
}
