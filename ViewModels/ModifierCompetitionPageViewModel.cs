namespace Chess_D_B.ViewModels;

public partial class ModifierCompetitionPageViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    
    public ModifierCompetitionPageViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        
    }
}