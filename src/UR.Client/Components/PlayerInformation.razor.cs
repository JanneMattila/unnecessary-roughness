using Microsoft.AspNetCore.Components;
using UR.Data;

namespace UR.Client.Components;

public class PlayerInformationBase : ComponentBase
{
    [Parameter]
    public Player? SelectedPlayer { get; set; }
}
