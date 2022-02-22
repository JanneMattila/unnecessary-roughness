using Microsoft.AspNetCore.Components;

namespace UR.Client.Components;

public class DiceBase : ComponentBase
{
    [Parameter]
    public string Number { get; set; } = string.Empty;
}
