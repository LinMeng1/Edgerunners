   M S B A M L   `   `   `  ����I  ENightCity.Core, Version=1.0.0.1, Culture=neutral, PublicKeyToken=null]?clr-namespace:NightCity.Core.Converters;assembly=NightCity.CoreNightCity.Core.Converters  5      D @NightCity, Version=1.0.0.7, Culture=neutral, PublicKeyToken=null;"clr-namespace:NightCity.ConvertersNightCity.Converters 5	      O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5
      #   NightCity.Views.ModuleManager   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      _ [MaterialDesignThemes.Wpf, Version=4.9.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5      T PPrism.Wpf, Version=8.1.97.5141, Culture=neutral, PublicKeyToken=40ee6c3a2184dc59[	 WPrism.DryIoc.Wpf, Version=8.1.97.5141, Culture=neutral, PublicKeyToken=40ee6c3a2184dc59&prismhttp://prismlibrary.com/  	 5      _
 [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a3i+http://schemas.microsoft.com/xaml/behaviors 
 5      O	converter?clr-namespace:NightCity.Core.Converters;assembly=NightCity.Core 
 5      7converterLocal"clr-namespace:NightCity.Converters 
 5	      3system&clr-namespace:System;assembly=mscorlib 
 5
      !  Prism.Mvvm.ViewModelLocator    AutoWireViewModel$
  TrueX�5      $	��800��5      $	��400��6    � 	Resources 5      1 +NightCity.Converters.ModuleManagerConverter   ModuleManagerConverter5   /NightCity.Core.Converters.MultiBindingConverter  MultiBindingConverterA   ;NightCity.Core.Converters.ObjectIsNullToVisibilityConverter % !ObjectIsNullToVisibilityConverter=   7NightCity.Core.Converters.DialogHostVisibilityConverter ! DialogHostVisibilityConverter1   +NightCity.Core.Converters.WrapTextConverter  WrapTextConverter  Card �� 
TargetType#  MaterialDesignThemes.Wpf.Card �� Property �� Value  PackIcon'  !MaterialDesignThemes.Wpf.PackIcon�� 5   
   %,  &	        &	      &	 &     &	 9     &	 L     &	 _     &	 *      5      6U     5      6N     5      6f     5      6^     5      6F   �� 5      "  6!   .�5      ��  �@ UniformCornerRadius$ 5Q�68   �� 5       � ��$ 5q�6,   �� 5       � ��$ 5q�6+   �� 5       � �� �#EEEEEE6/   5      �� 5      "  6%   .�5      ��  � ��$ 15��6*   �� 5       � ��$ 15��6+   �� 5       � ��$ Center=�68   �� 5       � ��$ Center=�66   5      5       .��5#      )	  #MaterialDesignThemes.Wpf.DialogHost	   $  MaterialDesignEmbeddedDialogHost#��[ 6J    	  IsOpen 6    �� *MessageHost.DialogOpen+ 	  DialogContent 5$   
   �� 5%       �� CornerRadius$ 8��6    J� Effect 5&      3
  -System.Windows.Media.Effects.DropShadowEffect
  5'      	 
  
BlurRadius$	 20Q�6'   
 
  ShadowDepth$
 0Q�67    
  Color$ Black��6G    
  Opacity$	 0.4Q�6U   ._�5)        .��5*      �� $	��600��6!   .+�5+      �� $��16q�6%   .+�5,      �  � IsIndeterminate .  % !MaterialDesignCircularProgressBar#��[ 6A    � Value$ 0Q�6|   �� 5-      $��Syncing data��6(   6>   y�5.      `� 5/   "    `� 	Converter# [ 6/   .1�50   &   ��  �� Path$  MessageHost.DialogCategory�6.   6R    `� ConverterParameter$ Syncing��51   o   52   #   54      �� 55      $��16q�6%   .+�56      �� $
��Wrap=�6S   ��6(   �� *MessageHost.DialogMessage+6S   �� 57      $��0,8,8,0q�6%   $��I Know��6^    	 MaterialDesignFlatButton#��[	 6o   ��66   �� *CleanMessageCommand+6o   y�58      `� 59   "   # [ 6/   .1�5:   &   �� $  MessageHost.DialogCategory�6.   6R   $ Message��5;   o   5<   #   5>      �� 5?      $��16q�6%   .+�5@      �� ��6(   �� *MessageHost.DialogMessage+� 5A       � ColumnDefinitions 5B   "   �� 5C   &   $
��auto �67   �� 5D   &   $��* �67   �� 5E   &   $
��auto �67   .+�5G   "   �� $��0��6)   $��0,16,0,0q�69   $��ACCEPT��6v   #��[	 6�   ��6K   �� *UninstallModuleCommand+6�   �� 5H   "   $��2��6)   $��0,16,0,0q�69   $��CANCEL��6s   #��[	 6�   ��6K   �� *CleanMessageCommand+6�   5I      y�5J      `� 5K   "   # [ 6/   .1�5L   &   �� $  MessageHost.DialogCategory�6.   6R   $ 	Uninstall��5M   q   5N   #   5P      �� 5Q      $��16q�6%   .+�5R      �� $
��Auto=�6+   $	��260��6N   .��5S   "   �� $
��Wrap=�6m   ��6,   �� *1/InstalledSelectedModuleSelectedVersion.Manifest+6m   5T      �� 5U      $��0,8,8,0q�6%   $��	Installed��6m   $"��Got It and Copy to Clipboard��6�   #��[	 6�   ��66   �� *$"ClearMessageAndCopyManifestCommand+6�   y�5V      `� 5W   "   # [ 6/   .1�5X   &   �� $  MessageHost.DialogCategory�6.   6R   $ Manifest Installed��5Y   z   5Z   #   5\      �� 5]      $��16q�6%   .+�5^      �� $
��Auto=�6+   $	��260��6N   .��5_   "   �� $
��Wrap=�6j   ��6,   �� *.,BrowseSelectedModuleSelectedVersion.Manifest+6j   5`      �� 5a      $��0,8,8,0q�6%   $��Browse��6m   $"��Got It and Copy to Clipboard��6�   #��[	 6�   ��66   �� *$"ClearMessageAndCopyManifestCommand+6�   y�5b      `� 5c   "   # [ 6/   .1�5d   &   �� $  MessageHost.DialogCategory�6.   6R   $ Manifest Browse��5e   w   5f   #   5h      5i      5j      5k      .��5m   
   � .+�5n      �� $��10q�6    �� SelectedIndex 6%   �� *TabSelectedIndex+.@�5o      �� $��	Installed��6   $��5q�6-   $��30��69   .��5p      � $��0,10,0,0q�6    5q      �� 5r      $	��390 �6/   �� 5s      $��* �6/   .+�5u      � $��0��6    � RowDefinitions 5v      �� 5w   "   $
��auto �60   �� 5x   "   $��* �60   .+�5z        $��0��62   #��[ 6?   $��0q�6]   .��5{   "   �  5|   &   �� 5}   *   $��* �6;   �� 5~   *   $
��auto �6;   .+�5�   &   �� -   5        $��0��5�   .    
 MaterialDesignTextBox#��[
 6>   $��Center=�6�   $��5,10,0,5q�6�   
������ .  (MaterialDesignThemes.Wpf.TextFieldAssist   HasClearButton . ��6m   �� *InstalledModulesFilterText+ �� UpdateSourceTrigger$ PropertyChanged=�.��6T  * 
 $Microsoft.Xaml.Behaviors.Interaction   Triggers 5�   *   + 
 %Microsoft.Xaml.Behaviors.EventTrigger  5�   .      	EventName$ TextChanged��6=   * 
 $Microsoft.Xaml.Behaviors.TriggerBase   Actions. 5�   2   2 
 ,Microsoft.Xaml.Behaviors.InvokeCommandAction     Command 6H   �� *FilterInstalledModulesCommand+5�   /   .��5�   B   5�   '     5�   &   $��1��6>   $��7q�6N      Kind$
 Sync=�6Y    � Cursor$
 Hand��6e   
������ $��Center=�6�    5�   *     5�   .   $ 	MouseDown��6=   . 5�   2      6H   �� *SyncInstalledModulesCommand+5�   /   5�   '   5�   #   5�        5�      $��1��62   #��[ 6?   .��5�   "   �� $
��Auto=�6/   .��5�   &   �� ��63   �� *InstalledModules+��5�   *   �� 5�   .   .R�5�   2   ��  ��Name$ Border��69    �� Padding$ 8,2,8,2q�6I   $
 Hand��6[   $ 5��6i    5�   6     5�   :   $ 	MouseDown��6I   . 5�   >      6T   �� $- 'DataContext.InstalledModuleClickCommand� �� RelativeSource ��   �� Mode$  FindAncestor=�! �� AncestorType$! UserControlL�"   CommandParameter" 6�   �� *.+5�   ;   ._�5�   6   �  5�   :   �� 5�   >   $
��auto �6O   �� 5�   >   $��* �6O   �� 5�   >   $
��auto �6O   .+�5�   :     #  Name$
# Icon��6b   $��0��6R   $��30��6�   $��30��6�   $��Center=�6�   $��Center=�6�    6p   �� *Icon+6�   �� 5�   :   $��1��6E   $��8,0,0,0q�6U   .+�5�   >   �  5�   B   �� 5�   F   $
��auto �6W   �� 5�   F   $
��auto �6W   �� 5�   F   $
��auto �6W   �� 5�   F   $
��auto �6W   �� 5�   F   $��* �6W   .+�5�   B     $# Official��6j   $��0��6Z   $
 Seal=�6|   $��Center=�6�   $y�	Collapsed=�6�   �� 5�   B   $ ��Name$
$ Name��6\   $��1��6L   $��2,0,0,0q�6j   
���   �$��Black"�6�   $��13(�6�   ��6{   �� *Name+6�   �� 5�   B   $$ Author��6\   $��2��6L   $��8,0,0,0q�6l   
���   �$��Black"�6�   $��Center=�6�   ��6}   �� *Author+6�   �� 5�   B   $$ at��6\   $��3��6L   $��0,0,0,0q�6h   $��@��6y   $��Center=�6�   �� 5�   B   $$ AuthorItCode��6\   $��4��6L   
���   �$��Black"�6�   $��Center=�6�   ��6r   �� *AuthorItCode+6�   5�   ?   �� 5�   >   $$ Description��6H   $��WordEllipsis=�6z   ��6]   �� *Description+6z   5�   ;   �� 5�   :   $��2��6E   .+�5�   >   �� $$ Version��6H   ��6Y   �� *	Version+  5�   >   $# Remove��6d   $ Remove=�6V   $��Right=�6t   
������ ���#C62828$y�	Collapsed=�6�    5�   B     5�   F   $ 	MouseDown��6U   . 5�   J      6`   �� $+ %DataContext.TryUninstallModuleCommand� �� $  FindAncestor=�$! UserControlL�5�   G   5�   ?   5�   ;   5�   7   5�   3   % �� Triggers% 5�   2   P� 5�   6   & P� Property& �@J�IsMouseOver' P� Value' . . �5�   :   �� ( �� 
TargetName$( Border��6A    � �� �#E0E0E06k   5�   7   �� 5�   6   ) �� Value$
) True��6B   * �� Binding* 5�   :   `� 5�   >   # [  6K   .1�5�   B   �� $. (DataContext.InstalledSelectedModule.Name�6J    6z   �� $  FindAncestor=�$! UserControlL�6�   �� 5�   B   $
 Name�6J   $ 
IsSelected��5�   �   5�   ?   .a�5�   :   �� $( Remove��6A    � y�$ Visible=�6k   �� 5�   :   $( Border��6A    � �� �#7575756k   �� 5�   :   $
( Icon��6A    � �� �#FFFFFF6i   �� 5�   :   $
( Name��6A    � �� �#FFFFFF6i   �� 5�   :   $( Author��6A    � �� �#FFFFFF6k   �� 5�   :   $( at��6A    � �� �#FFFFFF6g   �� 5�   :   $( AuthorItCode��6A    � �� �#FFFFFF6q   �� 5�   :   $( Description��6A    � �� �#FFFFFF6p   �� 5�   :   $( Version��6A    � �� �#FFFFFF6l   �� 5�   :   $( Official��6A    � �� �#FFFFFF6m   5�   7   �� 5�   6   $
) True��6a   * 6B   �� *
IsOfficial+.a�5�   :   �� $( Official��6A    � y�$ Visible=�6m   5�   7   �� 5�   6   $) False��6`   * 6B   �� *	IsVisible+.a�5�   :   �� $( Border��6A    � y�$ 	Collapsed=�6k   5�   7   5�   /   5�   '   5�   #   5�      5�      � 5�      $��1��6   .+�5�      �  5�   "   �� 5�   &   $
��auto �64   �� 5�   &   .+�5�   "   � $��0��6'   $  ��64    5�   &   �� 5�   *   �� 5�   *   $
��auto �6;   .+�5�   &   �� $��0��6/   $��6q�6?   $��6q�6K   )  #MaterialDesignThemes.Wpf.HintAssist+   Hint$+ Module Version��6V   , �� DisplayMemberPath$, Version��6�    " MaterialDesignOutlinedComboBox#��[ 6  ��6�   �� *!InstalledSelectedModuleVersions+- �� SelectedItem- 6�   �� *(&InstalledSelectedModuleSelectedVersion+6  �� 5�   &    " MaterialDesignRaisedDarkButton#��[ 6-   $��60��6e   ���#1E88E5$��Update��6�   $��10(�6�   $��1��6�   $��5q�6�   $��5q�6�   $
 Hand��6�   ��6�   �� *UpdateModuleCommand+��5�   *   `� 5�   .   # [ 6;   .1�5�   2   �� $" InstalledSelectedModule.Name�6:   6`   �� 5�   2   $4 .InstalledSelectedModuleSelectedVersion.Version�6:   6r   �� 5�   2   $&  InstalledSelectedModule.Category�6:   6d   5�   /   y�5�   *   `� 5�   .   # [  6;   .1�5�   2   �� . �� Source. 5�   6   ��5�   :   
Update��6P   5�   3   �� 5�   2   $" InstalledSelectedModule.Name�6:   6`   �� 5�   2   $4 .InstalledSelectedModuleSelectedVersion.Version�6:   6r   �� 5   2   $ InstalledModules�6:   6T   �� 5  2   $ LoadedModules�6:   6Q   $ ActionButtonVisibility��5  �   5  /   5  '   �� 5  &   #��[ 6-   $��60��6e   $��Launch��6p   $��10(�6�   $��1��6�   $��5q�6�   $��5q�6�   ��6�   �� *LaunchModuleCommand+��6�   �� *InstalledSelectedModule.Name+y�5  *   `� 5	  .   # [  6;   .1�5
  2   �� . 5  6   ��5  :   
Launch��6P   5  3   �� 5  2   $" InstalledSelectedModule.Name�6:   6`   �� 5  2   $4 .InstalledSelectedModuleSelectedVersion.Version�6:   6r   �� 5  2   $ InstalledModules�6:   6T   �� 5  2   $ LoadedModules�6:   6Q   $ ActionButtonVisibility��5  �   5  /   5  '   �� 5  &   #��[ 6-   $��60��6e   
���   ����#FFCA28$��Reload��6�   $��10(�6�   $��1��6�   $��5q�6�   $��5q�6�   ��6�   �� *ReloadNightCityCommand+y�5  *   `� 5  .   # [  6;   .1�5  2   �� . 5  6   ��5  :   
Reload��6P   5  3   �� 5   2   $" InstalledSelectedModule.Name�6:   6`   �� 5!  2   $4 .InstalledSelectedModuleSelectedVersion.Version�6:   6r   �� 5"  2   $ InstalledModules�6:   6T   �� 5#  2   $ LoadedModules�6:   6Q   $ ActionButtonVisibility��5$  �   5%  /   5'  '   5(  #     5)  "   $��1��66   #��[ 6C   .��5*  &   �  5+  *   �� 5,  .   $
��auto �6<   �� 5-  .   $
��auto �6<   �� 5.  .   $
��auto �6<   �� 5/  .   $
��auto �6<   �� 50  .   $
��auto �6<   �� 51  .   $��* �6<   .+�53  *   �� $��0��65   .+�54  .   �� $��Description��68   
���   �$��Black"�6^   $��13(�6q   �� 55  .   $
��Wrap=�6�   ��68   �� *%#InstalledSelectedModule.Description+/ �� 	Converter#/ [ 6�   56  +   �� 57  *   $��1��64     MaterialDesignDarkSeparator#��[ 6A   �� 58  *   $��2��65   .+�59  .   �� 0 �� Orientation$0 
Horizontal=�69   .+�5:  2   �� $��Author:��6<   
���   �$��Black"�6^   $��13(�6q   �� 5;  2   $��Center=�6l   $��10,0,0,0q�6�   ��6<   �� * InstalledSelectedModule.Author+6�   5<  /   �� 5=  .   $0 
Horizontal=�69   .+�5>  2   �� $��	Official:��6<   
���   �$��Black"�6`   $��13(�6s   �� 5?  2   $��Center=�6p   $��10,0,0,0q�6�   ��6<   �� *$"InstalledSelectedModule.IsOfficial+6�   5@  /   �� 5A  .   $0 
Horizontal=�69   .+�5B  2   �� $��Release Time:��6<   
���   �$��Black"�6d   $��13(�6w   �� 5C  2   $��Center=�6�   $��10,0,0,0q�6�   ��6<   �� *42InstalledSelectedModuleSelectedVersion.ReleaseTime+1 �� StringFormat$1 yyyy-MM-dd HH:mm:ss��6�   5D  /   �� 5E  .   $0 
Horizontal=�69   .+�5F  2   �� $��	Manifest:��6<   
���   �$��Black"�6`   $��13(�6s     5G  2   $ CodeJson=�6J   $��10,0,0,0q�6Z   #��[ 6l   $
 Hand��6�   
������ $��Right=�6�   y�6�   �� *1/InstalledSelectedModuleSelectedVersion.Manifest+#/ [  5H  6     5I  :   $ 	MouseDown��6I   . 5J  >     $" 	Installed��6�    6T   �� *ShowModuleManifestCommand+6�   5K  ;   5M  3   5N  /   5O  +   �� 5P  *   $��3��64   #��[ 6A   �� 5Q  *   $��4��64   $��Release Notes:��6A   
���   �$��Black"�6j   $��13(�6}   �� 5R  *   $
��Auto=�67   $��5��6Z   .��5S  .   �� $
��Wrap=�6�   ��68   �� *53InstalledSelectedModuleSelectedVersion.ReleaseNotes+#/ [ 6�   5T  +   5U  '   5V  #   5W     5X     5Y     5Z     �� 5[     $��Browse��6   $��5q�6*   $��30��66   .��5\     � $��0,10,0,0q�6    5]     �� 5^     $	��390 �6/   �� 5_     $��* �6/   .+�5a     � $��0��6    5b     �� 5c  "   $
��auto �60   �� 5d  "   $��* �60   .+�5f       $��0��62   #��[ 6?   $��0q�6]   .��5g  "   �  5h  &   �� 5i  *   $
��auto �6;   �� 5j  *   $��* �6;   �� 5k  *   $
��auto �6;   .+�5m  &     $
 Seal=�6>   $��0��6J   $��7q�6Z   $
 Hand��6e   
������  5n  *     5o  .   $ 	MouseDown��6=   . 5p  2      6H   �� *(&SwitchBrowseModulesOfficialOnlyCommand+5q  /   ��5s  *   �� 5t  .   "  64   2 �� BasedOn#2 [ 6Y   .�5u  2   ��  � ��
 �   �6O   3 �� Triggers3 5v  2   �� 5w  6   * 6B   �� *BrowseModulesFilterIsOfficial+	) False6t   .a�5x  :   ��  � ��
 �����6W   5y  7   5{  /   5}  '   �� 5~  &   -   5        $��1��5~  .   #��[
 6>   $��Center=�6�   $��0,10,0,5q�6�   
������  . ��6m   �� *BrowseModulesFilterText+$ PropertyChanged=�.��6Q   5  *     5�  .   $ TextChanged��6=   . 5�  2      6H   �� *FilterBrowseModulesCommand+5�  /   .��5�  B   5�  '     5�  &   $��2��6>   $��7q�6N   $
 Sync=�6Y   #��[ 6e   $
 Hand��6�   
������  5�  *     5�  .   $ 	MouseDown��6=   . 5�  2      6H   �� *SyncBrowseModulesCommand+5�  /   5�  '   5�  #   5�       5�     $��1��62   #��[ 6?   .��5�  "   �� $
��Auto=�6/   .��5�  &   �� ��63   �� *BrowseModules+��5�  *   �� 5�  .   .R�5�  2   �� $ Border��69   $ 8,2,8,2q�6I   $
 Hand��6[   $ 5��6i    5�  6     5�  :   $ 	MouseDown��6I   . 5�  >      6T   �� $* $DataContext.BrowseModuleClickCommand� �� $  FindAncestor=�$! UserControlL�" 6�   �� *.+5�  ;   ._�5�  6   �  5�  :   �� 5�  >   $
��auto �6O   �� 5�  >   $��* �6O   �� 5�  >   $
��auto �6O   .+�5�  :     $
# Icon��6b   $��0��6R   $��30��6�   $��30��6�   $��Center=�6�   $��Center=�6�    6p   �� *Icon+6�   �� 5�  :   $��1��6E   $��8,0,0,0q�6U   .+�5�  >   �  5�  B   �� 5�  F   $
��auto �6W   �� 5�  F   $
��auto �6W   �� 5�  F   $
��auto �6W   �� 5�  F   $
��auto �6W   �� 5�  F   $��* �6W   .+�5�  B     $# Official��6j   $��0��6Z   $
 Seal=�6|   $��Center=�6�   $y�	Collapsed=�6�   �� 5�  B   $
$ Name��6\   $��1��6L   $��2,0,0,0q�6j   
���   �$��Black"�6�   $��13(�6�   ��6{   �� *Name+6�   �� 5�  B   $$ Author��6\   $��2��6L   $��8,0,0,0q�6l   
���   �$��Black"�6�   $��Center=�6�   ��6}   �� *Author+6�   �� 5�  B   $$ at��6\   $��3��6L   $��0,0,0,0q�6h   $��@��6y   $��Center=�6�   �� 5�  B   $$ AuthorItCode��6\   $��4��6L   
���   �$��Black"�6�   $��Center=�6�   ��6r   �� *AuthorItCode+6�   5�  ?   �� 5�  >   $$ Description��6H   $��WordEllipsis=�6z   ��6]   �� *Description+6z   5�  ;   �� 5�  :   $��2��6E   .+�5�  >     $# IsInstalled��6V   $ CheckCircle=�6k   5�  ;   5�  7   5�  3   % 5�  2   P� 5�  6   & �@J�IsMouseOver' . . �5�  :   �� $( Border��6A    � �� �#E0E0E06k   5�  7   �� 5�  6   $
) True��6B   * 5�  :   `� 5�  >   # [  6K   .1�5�  B   �� $+ %DataContext.BrowseSelectedModule.Name�6J    6w   �� $  FindAncestor=�$! UserControlL�6�   �� 5�  B   $
 Name�6J   $ 
IsSelected��5�  �   5�  ?   .a�5�  :   �� $( Border��6A    � �� �#7575756k   �� 5�  :   $
( Icon��6A    � �� �#FFFFFF6i   �� 5�  :   $
( Name��6A    � �� �#FFFFFF6i   �� 5�  :   $( Author��6A    � �� �#FFFFFF6k   �� 5�  :   $( at��6A    � �� �#FFFFFF6g   �� 5�  :   $( AuthorItCode��6A    � �� �#FFFFFF6q   �� 5�  :   $( Description��6A    � �� �#FFFFFF6p   �� 5�  :   $( IsInstalled��6A    � �� �#FFFFFF6p   �� 5�  :   $( Official��6A    � �� �#FFFFFF6m   5�  7   �� 5�  6   $) False��6B   * 5�  :   `� 5�  >   # [  6K   .1�5�  B   �� $
 Name�6J   �� 5�  B   $" DataContext.InstalledModules�6J    6n   �� $  FindAncestor=�$! UserControlL�6�   $ IsInstalled��5�  �   5�  ?   .a�5�  :   �� $( IsInstalled��6A    � y�$ 	Collapsed=�6p   5�  7   �� 5�  6   $
) True��6a   * 6B   �� *
IsOfficial+.a�5�  :   �� $( Official��6A    � y�$ Visible=�6m   5�  7   �� 5�  6   $) False��6`   * 6B   �� *	IsVisible+.a�5�  :   �� $( Border��6A    � y�$ 	Collapsed=�6k   5�  7   5�  /   5�  '   5�  #   5�     5�     � 5�     $��1��6    5�     �� 5�  "   $
��auto �60   �� 5�  "   .+�5�     � $��0��6#   $  ��60    5�  "   �� 5�  &   �� 5�  &   $
��auto �67   .+�5�  "   �� $��0��6+   $��6q�6;   $��6,6,6,6q�6G   $+ Module Version��6X   $, Version��6�   #��[ 6  ��6�   �� *BrowseSelectedModuleVersions+- 6�   �� *%#BrowseSelectedModuleSelectedVersion+6  �� 5�  "   #��[ 6)   $��60��6a   ���#00897B$��Install��6�   $��10(�6�   $��1��6�   $��5q�6�   $��5q�6�   $
 Hand��6�   ��6�   �� *UpdateModuleCommand+��5�  &   `� 5�  *   # [ 67   .1�5�  .   �� $ BrowseSelectedModule.Name�66   6Y   �� 5�  .   $1 +BrowseSelectedModuleSelectedVersion.Version�66   6k   �� 5�  .   $# BrowseSelectedModule.Category�66   6]   5�  +   y�5�  &   `� 5�  *   # [  67   .1�5�  .   �� . 5�  2   ��5�  6   Install��6M   5�  /   �� 5�  .   $ BrowseSelectedModule.Name�66   6Y   �� 5�  .   $1 +BrowseSelectedModuleSelectedVersion.Version�66   6k   �� 5�  .   $ InstalledModules�66   6P   �� 5   .   $ LoadedModules�66   6M   $ ActionButtonVisibility��5  �   5  +   5  #   �� 5  "   #��[ 6)   $��60��6a   ���#1E88E5$��Update��6�   $��10(�6�   $��1��6�   $��5q�6�   $��5q�6�   ��6�   �� *UpdateModuleCommand+��5  &   `� 5  *   # [ 67   .1�5  .   �� $ BrowseSelectedModule.Name�66   6Y   �� 5	  .   $1 +BrowseSelectedModuleSelectedVersion.Version�66   6k   �� 5
  .   $# BrowseSelectedModule.Category�66   6]   5  +   y�5  &   `� 5  *   # [  67   .1�5  .   �� . 5  2   ��5  6   
Update��6L   5  /   �� 5  .   $ BrowseSelectedModule.Name�66   6Y   �� 5  .   $1 +BrowseSelectedModuleSelectedVersion.Version�66   6k   �� 5  .   $ InstalledModules�66   6P   �� 5  .   $ LoadedModules�66   6M   $ ActionButtonVisibility��5  �   5  +   5  #   �� 5  "   #��[ 6)   $��60��6a   $��Launch��6l   $��10(�6}   $��1��6�   $��5q�6�   $��5q�6�   $
 Hand��6  ��6�   �� *LaunchModuleCommand+��6�   �� *BrowseSelectedModule.Name+y�5  &   `� 5  *   # [  67   .1�5  .   �� . 5   2   ��5!  6   
Launch��6L   5#  /   �� 5$  .   $ BrowseSelectedModule.Name�66   6Y   �� 5%  .   $1 +BrowseSelectedModuleSelectedVersion.Version�66   6k   �� 5&  .   $ InstalledModules�66   6P   �� 5'  .   $ LoadedModules�66   6M   $ ActionButtonVisibility��5(  �   5)  +   5+  #   �� 5,  "   #��[ 6)   $��60��6a   
���   ����#FFCA28$��Reload��6�   $��10(�6�   $��1��6�   $��5q�6�   $��5q�6�   ��6�   �� *ReloadNightCityCommand+y�5-  &   `� 5.  *   # [  67   .1�5/  .   �� . 50  2   ��51  6   
Reload��6L   53  /   �� 54  .   $ BrowseSelectedModule.Name�66   6Y   �� 55  .   $1 +BrowseSelectedModuleSelectedVersion.Version�66   6k   �� 56  .   $ InstalledModules�66   6P   �� 57  .   $ LoadedModules�66   6M   $ ActionButtonVisibility��58  �   59  +   5;  #   5<       5=     $��1��62   #��[ 6?   .��5>  "   �  5?  &   �� 5@  *   $
��auto �68   �� 5A  *   $
��auto �68   �� 5B  *   $
��auto �68   �� 5C  *   $
��auto �68   �� 5D  *   $
��auto �68   �� 5E  *   $��* �68   .+�5G  &   �� $��0��61   .+�5H  *   �� $��Description��64   
���   �$��Black"�6Z   $��13(�6m   �� 5I  *   $
��Wrap=�6�   ��64   �� *" BrowseSelectedModule.Description+#/ [ 6�   5J  '   �� 5K  &   $��1��60   #��[ 6=   �� 5L  &   $��2��61   .+�5M  *   �� $0 
Horizontal=�65   .+�5N  .   �� $��Author:��68   
���   �$��Black"�6Z   $��13(�6m   �� 5O  .   $��Center=�6e   $��10,0,0,0q�6�   ��68   �� *BrowseSelectedModule.Author+6�   5P  +   �� 5Q  *   $0 
Horizontal=�65   .+�5R  .   �� $��	Official:��68   
���   �$��Black"�6\   $��13(�6o   �� 5S  .   $��Center=�6i   $��10,0,0,0q�6�   ��68   �� *!BrowseSelectedModule.IsOfficial+6�   5T  +   �� 5U  *   $0 
Horizontal=�65   .+�5V  .   �� $��Release Time:��68   
���   �$��Black"�6`   $��13(�6s   �� 5W  .   $��Center=�6�   $��10,0,0,0q�6�   ��68   �� *1/BrowseSelectedModuleSelectedVersion.ReleaseTime+$1 yyyy-MM-dd HH:mm:ss��6�   5X  +   �� 5Y  *   $0 
Horizontal=�65   .+�5Z  .   �� $��	Manifest:��68   
���   �$��Black"�6\   $��13(�6o     5[  .   $ CodeJson=�6F   $��10,0,0,0q�6V   #��[ 6h   $
 Hand��6�   
������ $��Right=�6�   y�6�   �� *.,BrowseSelectedModuleSelectedVersion.Manifest+#/ [  5\  2     5]  6   $ 	MouseDown��6E   . 5^  :     $" Browse��6~    6P   �� *ShowModuleManifestCommand+6~   5_  7   5a  /   5b  +   5c  '   �� 5d  &   $��3��60   #��[ 6=   �� 5e  &   $��4��60   $��Release Notes:��6=   
���   �$��Black"�6f   $��13(�6y   �� 5f  &   $
��Auto=�63   $��5��6V   .��5g  *   �� $
��Wrap=�6�   ��64   �� *20BrowseSelectedModuleSelectedVersion.ReleaseNotes+#/ [ 6�   5h  '   5i  #   5j     5k     5l     5m     5n     5o     5p     5q     