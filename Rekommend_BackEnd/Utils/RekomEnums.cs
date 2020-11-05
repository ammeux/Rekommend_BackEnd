namespace Rekommend_BackEnd.Utils
{
    public class RekomEnums
    {
        public enum CompanyCategory
        {
            Ecology,
            LocalFood,
            Health,
            SilverEconomy
        }

        public enum JobTechLanguage
        {
            JavaScript,
            Python,
            CS,
            Java
        }

        public enum Position
        {
            TechLeader,
            Developer,
            BusinessAnalyst,
            ProductOwner,
            QA,
            Designer
        }

        public enum Seniority
        {
            Junior,
            Senior,
            Director,
            Executive
        }

        public enum ContractType
        {
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
            EmailToBeVerified,
            NotViewed,
            Viewed,
            Accepted,
            Rejected,
            Selected
        }

        public enum RecruiterPosition
        {
            Founder,
            TalentAcquisitionManager,
            Other
        }


    }
}
