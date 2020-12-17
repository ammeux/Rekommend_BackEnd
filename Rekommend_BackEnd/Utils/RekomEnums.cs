namespace Rekommend_BackEnd.Utils
{
    public class RekomEnums
    {
        public enum CompanyCategory
        { 
            Undefined,
            Ecology,
            LocalFood,
            Health,
            SilverEconomy
        }

        public enum JobTechLanguage
        {
            Undefined,
            JavaScript,
            Python,
            CS,
            Java
        }

        public enum Position
        {
            Undefined,
            TechLeader,
            Developer,
            BusinessAnalyst,
            ProductOwner,
            QA,
            Designer
        }

        public enum Seniority
        {
            Undefined,
            Junior,
            Senior,
            Director,
            Executive
        }

        public enum ContractType
        {
            Undefined,
            CDI,
            CDD,
            Internship,
            WorkStudy,
            Freelance
        }

        public enum JobOfferStatus
        {
            Open,
            ClosedFilledThanksToRekommend,
            ClosedFilledByAnotherCanal,
            ClosedNotFilled
        }

        public enum RekommendationStatus
        {
            Undefined,
            EmailToBeVerified,
            NotViewed,
            Viewed,
            Accepted,
            Rejected,
            Selected
        }

        public enum RecruiterPosition
        {
            Undefined,
            Founder,
            TalentAcquisitionManager,
            Other
        }

        public enum Country
        {
            Undefined,
            France,
            Other
        }

        public enum ResourceUriType
        {
            PreviousPage,
            NextPage,
            Current
        }

        public enum Gender
        {
            Undefined,
            Female,
            Male
        }
    }
}
