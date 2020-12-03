namespace LetsSport.Data.Models.Arenas
{
    using System.ComponentModel.DataAnnotations;

    public enum ArenaStatus
    {
        Active = 1,
        Inactive = 2,

        [Display(Name = "In Reconstruction")]
        InReconstruction = 3,

        [Display(Name = "Temporary Unavailable")]
        TemporatyUnavailable = 4,

        [Display(Name = "Opening Soon")]
        OpeningSoon = 5,
        Closed = 6,
    }
}
