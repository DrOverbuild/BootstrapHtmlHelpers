using System.ComponentModel.DataAnnotations;

namespace Web;

public class TheModelWithEverything
{
    [Display(Prompt = "Enter your name", Description = "We, like, really need your name!")]
    public string Name { get; set; } // basic text box

    public string Email { get; set; } // email text box

    public string Password { get; set; } // password box

    public DateTime? Birthday { get; set; } // date picker

    [Required]
    [Display(Name = "Can you even?", Description = "Select Yes if you can even. Select No if you can't even.")]
    [CantEven(ErrorMessage = "Come on man, you can't even get this right!")]
    public bool? CanYouEven { get; set; } // yes/no radio

    [Display(Name = "Again I ask, Can you even?", Description = "Repeat question")]
    [CantEven(ErrorMessage = "This must be true to continue!")]
    public bool CanYouEvenAgain { get; set; } // checkbox

    [Display(Description = "Do you agree?")]
    public SomeOptions SelectEnumQ { get; set; }

    [Display(Description = "Do you agree, but optional?")]
    [CantEven(ErrorMessage = "Cannot even")]
    public SomeOptions? SelectEnumNullableQ { get; set; }
    public ClassWithMoreStuff ClassWithMoreStuff { get; set; }
}

public class ClassWithMoreStuff
{
    [Display(Name = "Variable 1")]
    public string Var1 { get; set; }
}

public enum SomeOptions
{
    [Display(Name = "Strongly Disagree")]
    StronglyDisagree,
    Disagree,
    Neutral,
    Agree,
    [Display(Name = "Strongly Agree")]
    StronglyAgree
}