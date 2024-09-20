using System.ComponentModel.DataAnnotations;

namespace Web;

public class TheModelWithEverything
{
    [Display(Prompt = "Enter your name", Description = "We, like, really need your name!")]
    public string Name { get; set; } = null!;// basic text box

    public string Email { get; set; } = null!; // email text box

    public string Password { get; set; } = null!; // password box

    public DateTime? Birthday { get; set; } // date picker

    [Required]
    [Display(Name = "Can you even?", Description = "Select Yes if you can even. Select No if you can't even.")]
    [CantEven(ErrorMessage = "Come on man, you can't even get this right!")]
    public bool? CanYouEven { get; set; } // yes/no radio

    [Required]
    [Display(Name = "Not Nullable Yes No")]
    public bool NotNullableYesNo { get; set; } = true; // yes/no radio

    [Display(Name = "Again I ask, Can you even?", Description = "Repeat question")]
    [CantEven(ErrorMessage = "This must be true to continue!")]
    public bool CanYouEvenAgain { get; set; } // checkbox

    [Display(Description = "Do you agree?")]
    public SomeOptions SelectEnumQ { get; set; }

    [Display(Name = "Select all that apply", Description = "Then select more.")]
    [CantEven(ErrorMessage = "Come on man, you can't even get this right!")]
    public List<SomeOptions>? OptionsList { get; set; }

    [Display(Description = "Do you agree, but optional?")]
    [CantEven(ErrorMessage = "Cannot even")]
    public SomeOptions? SelectEnumNullableQ { get; set; }
    
    [Display(Description = "Nullable enum value with no initialized value and with empty first value text. Required so if the first option is still selected validation will fail.")]
    [Required]
    public SomeOptions? SelectEnumNullInitialValue  { get; set; }
    
    public ClassWithMoreStuff ClassWithMoreStuff { get; set; }

    public int CurrentPage { get; set; }
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