   M S B A M L   `   `   `  ����I  ENightCity.Core, Version=1.0.0.1, Culture=neutral, PublicKeyToken=null]?clr-namespace:NightCity.Core.Converters;assembly=NightCity.CoreNightCity.Core.Converters  5
      D @NightCity, Version=1.0.0.7, Culture=neutral, PublicKeyToken=null;"clr-namespace:NightCity.ConvertersNightCity.Converters 5      Y=clr-namespace:NightCity.Core.Controls;assembly=NightCity.CoreNightCity.Core.Controls  5          NightCity.Views.Connection   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      _ [MaterialDesignThemes.Wpf, Version=4.9.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5      T PPrism.Wpf, Version=8.1.97.5141, Culture=neutral, PublicKeyToken=40ee6c3a2184dc59[ WPrism.DryIoc.Wpf, Version=8.1.97.5141, Culture=neutral, PublicKeyToken=40ee6c3a2184dc59&prismhttp://prismlibrary.com/   5      _	 [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a3i+http://schemas.microsoft.com/xaml/behaviors 	 5	      O	converter?clr-namespace:NightCity.Core.Converters;assembly=NightCity.Core 	 5
      7converterLocal"clr-namespace:NightCity.Converters 	 5      Kcontrol=clr-namespace:NightCity.Core.Controls;assembly=NightCity.Core 	 5      !  Prism.Mvvm.ViewModelLocator    AutoWireViewModel$
  TrueX�5      $	��800��5      $	��500��6    � 	Resources 5      . (NightCity.Converters.ConnectionConverter   ConnectionConverter=   7NightCity.Core.Converters.DialogHostVisibilityConverter ! DialogHostVisibilityConverter  Card �� 
TargetType#  MaterialDesignThemes.Wpf.Card �� Property �� Value�� 5   
   %  &	        &	      &	 &       5      6O     5      6^   �� 5      "  6!   .�5      ��  �@ UniformCornerRadius$ 5Q�68   �� 5       � ��$ 5q�6,   �� 5       � ��$ 5q�6+   �� 5       � �� �#EEEEEE6/   5      5      .��5      )  #MaterialDesignThemes.Wpf.DialogHost   $  MaterialDesignEmbeddedDialogHost#��[ 6J      IsOpen 6    �� *MessageHost.DialogOpen+   DialogContent 5   
   �� 5       �� CornerRadius$ 8��6    J� Effect 5       3  -System.Windows.Media.Effects.DropShadowEffect  5!      	   
BlurRadius$	 20Q�6'   
   ShadowDepth$
 0Q�67      Color$ Black��6G      Opacity$	 0.4Q�6U   ._�5#        .��5$      �� $	��600��6!   .+�5%      �� $��16q�6%   .+�5&      �  � IsIndeterminate .  % !MaterialDesignCircularProgressBar#��[ 6A    � Value$ 0Q�6|   �� 5'      $��Syncing data��6(   6>   y�5(      `� 5)   "    `� 	Converter# [ 6/   .1�5*   &   ��  �� Path$  MessageHost.DialogCategory�6.   6R    `� ConverterParameter$ Syncing��5+   o   5,   #   5.      �� 5/      $��16q�6%   .+�50      �� $
��Wrap=�6S   ��6(   �� *MessageHost.DialogMessage+6S   �� 51      $��0,8,8,0q�6%   $��I Know��6^     MaterialDesignFlatButton#��[ 6o   ��66   �� *CleanMessageCommand+6o   y�52      `� 53   "   # [ 6/   .1�54   &   �� $  MessageHost.DialogCategory�6.   6R   $ Message��55   o   56   #   58      �� 59      $��16q�6%   .+�5:      �� $��Add to a cluster��6(   �� 5;      -   5        $��0,8,0,0q�5;   &   )  #MaterialDesignThemes.Wpf.HintAssist   Hint$ cluster name��67     MaterialDesignTextBox#��[ 6�   $��Stretch=�6�   ��6�   �� *EditingCluster+6�   �� 5<      $��With Category��6'   $��0,10,0,0q�6?    d� IsThreeState .   d� 	IsChecked 6f   �� *IsEditingClusterWithCategory+�� 5=      -   5        $��0,8,0,0q�5=   &   $ category��67   $��Stretch=�6�   ��6}   �� *EditingClusterCategory+.��6�   ��5>   "   �� 5?   &   " ��6,    �� BasedOn# [ 6A    �� Triggers 5@   *   �� 5A   .    �� Binding 6:   �� *IsEditingClusterWithCategory+ �� Value	 False6k   .a�5B   2   ��  � y�$ 	Collapsed=�6O   5C   /   5E   '   .��5F   1   5G      �� 5H      $��Right=�6)    �� Orientation$ 
Horizontal=�6E   .+�5I   "   �� $��0,8,8,0q�6)   $��ACCEPT��6b   #��[ 6s   ��6:   �� *AddToClusterCommand+6s   �� 5J   "   $��0,8,8,0q�6)   $��CANCEL��6b   #��[ 6s   ��6:   �� *CleanMessageCommand+6s   5K      y�5L      `� 5M   "   # [ 6/   .1�5N   &   �� $  MessageHost.DialogCategory�6.   6R   $ AddToCluster��5O   t   5P   #   5R      �� 5S      $��16q�6%   .+�5T      �� ��6(   �� *MessageHost.DialogMessage+� 5U       � ColumnDefinitions 5V   "   �� 5W   &   $
��auto �67   �� 5X   &   $��* �67   �� 5Y   &   $
��auto �67   .+�5[   "   �� $��0��6)   $��0,16,0,0q�69   $��ACCEPT��6x   #��[ 6�   ��6K   �� *RemoveFromClusterCommand+6�   �� 5\   "   $��2��6)   $��0,16,0,0q�69   $��CANCEL��6s   #��[ 6�   ��6K   �� *CleanMessageCommand+6�   5]      y�5^      `� 5_   "   # [ 6/   .1�5`   &   �� $  MessageHost.DialogCategory�6.   6R   $ RemoveFromCluster��5a   y   5b   #   5d      5e      5f      5g      .��5i   
   �  � RowDefinitions 5j      �� 5k      $
��auto �6    �� 5l      $��* �6    .+�5n        $��0��6"   #��[ 6/   .��5o      �  5p      �� 5q      $
��auto �6+   �� 5r      $��* �6+   �� 5s      $
��auto �6+   �� 5t      $
��auto �6+   .+�5v      d�  $  MaterialDesignSwitchToggleButton#��[ 6#    6]   �� *IsFix+.��5w      '  !MaterialDesignThemes.Wpf.PackIcon     Kind$
 Lock=�62    J� RenderTransformOrigin$ .5,.5&�6>   5x      1	  +MaterialDesignThemes.Wpf.ToggleButtonAssist 	  	OnContent 5y        5z      $
 Lock=�66   5|        5}      $��2��6.   $ CommentPlus=�6>    � Cursor$
 Hand��6Q   
������ $��0,0,10,0q�6x   *
 	 $Microsoft.Xaml.Behaviors.Interaction  
  Triggers  5~      + 	 %Microsoft.Xaml.Behaviors.EventTrigger  5      !   	EventName$! 	MouseDown��6-   * 	 $Microsoft.Xaml.Behaviors.TriggerBase"   Actions." 5�   "   2 	 ,Microsoft.Xaml.Behaviors.InvokeCommandAction  #   Command# 68   �� *TryAddToClusterCommand+5�      5�        5�      $��3��6.   $
 Sync=�6>   $
 Hand��6J   
������   5�        5�      $! 	MouseDown��6-   ." 5�   "     # 68   �� *SyncClustersCommand+5�      5�      5�      5�      � 5�      $��1��6    5�      �� 5�      $	��200 �6'   �� 5�      $��* �6'   .+�5�        $��0��6&   #��[ 63   .��5�      �� ��6   �� *TopicCollection.Topics+$ �� SelectedItem$ 6N   �� *SelectedTopic+9   3NightCity.Core.Controls.MouseLeftButtonDownBehavior%   Command% 6v   �� *ReadMessageCommand+��5�      �� 5�      " ��6$     MaterialDesignListBoxItem# [ 6>   .�5�   "   ��  � ��$ Stretch=�6O   5�      ��5�      �� 5�      .R�5�   "   �  5�   &   �� 5�   *   $
��auto �6;   �� 5�   *   $��* �6;   �� 5�   *   $
��auto �6;   .+�5�   &     &  Name$& NoClusterIcon��6>   $��0��6U   $	 Cog=�6e   
������ $��0,1,5,0q�6�   ' � ToolTip' 6�   �� *Origin+(  "MaterialDesignThemes.Wpf.ColorZone  5�   &   (  Name$( Category��6?   $��0,2,5,2q�6Q   )   CornerRadius$) 5��6c   *   Mode$* SecondaryMid=�6t   .��5�   *   �� $��8,0,8,0q�64   $��Center=�6E   $��11(�6`   ��6n   �� *
Category+5�   '   �� 5�   &   $��1��6G   $��Center=�6W   ��60   �� *Topic+6W   �� 5�   &   + ��Name$+ NoReadCount��6M   , �� Padding$, 5,0,5,0q�6-   $��20��6?   $��2��6b   $��1q�6r   ���#B71C1C$ 10��6�   $��Center=�6�   ._�5�   *   �� 
�������$��11(�6k   $��Center=�6y   ��6G   �� *NoReadMessageCount+6y   5�   '   5�   #   - �� Triggers- 5�   "   �� 5�   &   $ 0��6Y    62   �� *NoReadMessageCount+.a�5�   *   �� . �� 
TargetName$. NoReadCount��61    � y�$ 	Collapsed=�6`   5�   '   �� 5�   &   $
 True��62   .a�5�   *   �� $. NoClusterIcon��61    � y�$ 	Collapsed=�6b    5�   *   `� 5�   .   # [  6;   .1�5�   2   �� $ .�6:   $ IsClusterTopic��5�   �   5�   /   5�   '   �� 5�   &    62   �� *
Category+ 6O   Z� .a�5�   *   �� $. Category��61    � y�$ 	Collapsed=�6]   5�   '   5�      5�      5�      � 5�      $��1��6    5�      �� 5�      $
��auto �6(   �� 5�      $��* �6(   �� 5�      $
��auto �6(   .+�5�        $��0��6*   #��[ 67   .��5�      �  5�      �� 5�   "   $
��auto �63   �� 5�   "   $��* �63   �� 5�   "   $
��auto �63   �� 5�   "   $
��auto �63   .+�5�        $��0��66   $ Cancel=�6F   $
 Hand��6T   
������ ���#B71C1C$��0,0,10,0q�6�   ��5�   "   �� 5�   &   "  6,    5�   *   �� 5�   .   	 False6:    5�   2   `� 5�   6   # [  6C   .1�5�   :   �� $ SelectedTopic�6B   $ IsClusterTopic��5�   �   5�   7   .a�5�   2   ��  � y�$ 	Collapsed=�6O   5�   /   5�   '     5�   "     5�   &   $! 	MouseDown��65   ." 5�   *     # 6@   �� *TryRemoveFromClusterCommand+5�   '   5�        5�      $��2��66   $ Magnify=�6F   $��0,0,10,0q�6U     5�      $��3��66   $ Broom=�6F   $
 Hand��6S   
������   5�   "     5�   &   $! 	MouseDown��65   ." 5�   *     # 6@   �� *ClearMessageCommand+5�   '   5�      5�      5�        5�      $��1��6*   #��[ 67   .��5�      �� -   5        / ��Name$/ MessageScroll��5�   J   $
��Auto=�6'   .��5�      �� ��6+   �� *SelectedTopic.Messages+��5�   "   �� 5�   &   .R�5�   *   �� 0 ��Name$0 
MessageBox��6N   $ 
Horizontal=�65   $��0,0,0,10q�6b   .+�5�   .     $& Avatar��6F   $��20��6V   $��20��6b   $��10q�6m   $��0��6y   $ Account=�6�   � 5�   .    5�   2   �� 5�   6   �� 5�   6   �� 5�   6   .+�5�   2   �� $��0��6<   
�������$��LeftToRight=�6[   ��5�   6   `� 5�   :   1 �� StringFormat$1 {0}  {1:yyyy-MM-dd HH:mm}��6G   .1�5�   >   �� $ Sender�6F   �� 5�   >   $
 Time�6F   5�   ;   5  3     5  2   2  Name$2 Content��6F   $��1��6W   #��[ 6d   $��0,5,50,5q�6�   
�������$
��Left=�6�   .��5  6   �� 3 ��Name$
3 Text��6>   $��LeftToRight=�6L   
������ $��0q�6�   4 �� 
IsReadOnly4 . 5 �� TextWrapping$
5 Wrap=�6�   $	��500��6�   ��6�   �� *	Content+6�   5  3   �� 5  2   6 ��Name$6 Address��6I   $��2��6<   $��LeftToRight=�6[   
���������6w   �� *	Address+$1 from {0}��6�   5  /   5  +   - 5  *   �� 5	  .   $  ��6V    6:   �� *	Address+.a�5
  2   �� $. 
MessageBox��69    � ��$ RightToLeft=�6j   �� 5  2   $. Content��69    � �� �#C8E6C96d   �� 5  2   $. Address��69    � y�$ 	Collapsed=�6d   5  /   �� 5  .   $
 True��6[    6:   �� *IsMastermind+.a�5  2   �� $. Avatar��69    �@ Kind$ Brain=�6]   �� 5  2   $. Content��69    � �� �#01579B6d   �� 5  2   $
. Text��69    � ��
 �����6a   5  /   5  '   5     5     5       5     $��2��6*   #��[ 67   .��5     �  5     �� 5  "   �� 5  "   �� 5  "   .+�5      �� $ 
Horizontal=�6)   $��Right=�6B   $��0��6^   .+�5!  "   �� $��As Mastermind��6N    .  $��Right=�6{   {�.   6+   �� *IsMastermind+6�   5"     �� 5#     $��1��6(    	 MaterialDesignDarkSeparator#��[	 65   $��0q�6j   � 5$     $��2��6#    5%  "   �� 5&  &   $��* �67   �� 5'  &   $
��auto �67   .+�5)  "   �� -   5        $��0��5)  *   $��0,0,10,0q�6~   $��40��6�   $
5 Wrap=�6�   "7 �� VerticalScrollBarVisibility$
7 Auto=�6�   #��[ 6�   ��6:   �� *EditingMessage+8 �� UpdateSourceTrigger$8 PropertyChanged=�.��6  9 J� InputBindings9 5*  &   �� 5+  *   
: �� Key$: Enter��65   ; �� Command; 6A   �� *SendMessageCommand+.��5,  =   5-  #   �� 5.  "   $��1��6)   $��60��69   $
��Send��6D   $��10(�6S   $��5q�6a   $��5q�6m   $
 Hand��6�   ��6x   �� *SendMessageCommand+��5/  &   �� 50  *   " ��60    "
 MaterialDesignRaisedDarkButton# [
 6D    51  .   �� 52  2    6>   �� *IsConnected+	 False6^   .a�53  6   ��  � {� .  6R   54  3   �� 55  2    6>   �� *SelectedTopic+ 6`   Z� .a�56  6   ��  � {� .  6R   57  3   59  +   5;  #   5<     5=     5>     5?     5@     5A     5B     5C     