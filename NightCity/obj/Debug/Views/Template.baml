   M S B A M L   `   `   `  ����  	NightCity    NightCity.Views.Template   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   T PPrism.Wpf, Version=8.1.97.5141, Culture=neutral, PublicKeyToken=40ee6c3a2184dc59[ WPrism.DryIoc.Wpf, Version=8.1.97.5141, Culture=neutral, PublicKeyToken=40ee6c3a2184dc59&prismhttp://prismlibrary.com/   5   	   _ [MaterialDesignThemes.Wpf, Version=4.9.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5	   	   !  Prism.Mvvm.ViewModelLocator    AutoWireViewModel$
  TrueX�5   	   $	��580��5
   	   $	��800��6   ���#BDBDBD'  !System.Windows.Shell.WindowChrome   WindowChrome 5        5   
      GlassFrameThickness$ 0q�6      CornerRadius$ 10��6/      CaptionHeight$ 0Q�6A      ResizeBorderThickness$ 10q�6S   .��5      � $��5q�6    � RowDefinitions 5   
   �� 5      $
��auto �6   �� 5      $��* �6   �� 5      $
��auto �6   .+�5   
   #  MaterialDesignThemes.Wpf.Card  -   5        $��30��5      $��0��6*   
���   �.��5      �  � ColumnDefinitions 5      �� 5      $
��auto �6'   �� 5      $��* �6'   �� 5      $
��auto �6'   �� 5      $
��auto �6'   �� 5      $
��auto �6'   .+�5      '  !MaterialDesignThemes.Wpf.PackIcon  -   5          Name$ ApplicationIcon��5   *   $��0��6C   
�������$��22��6f   $��22��6r   $��10,4,0,4q�6}   �� 5       -   5        	 ��Name$
	 Head��5    ,   $��1��6   $��10,0,0,0q�6:   $
��Left=�6L   $��Center=�6g   
�������6�     5!      -   5        $��2��5!   &   
������ 
 � Cursor$

 Hand��6O   $��5q�6]   .��5"           Kind$ WindowMinimize=�6.   
�������$��20��6W   $��20��6c   $��Center=�6n   5#        5$      -   5        $��3��5$   &   
������ $

 Hand��6O   $��5q�6]   .��5%        $ WindowMaximize=�6.   
�������$��20��6W   $��20��6c   $��Center=�6n   5&        5'      -   5        $��4��5'   &   
������ $

 Hand��6O   $��5q�6]   .��5(        $ WindowClose=�6.   
�������$��20��6T   $��20��6`   $��Center=�6k   5)      5*      5+      � 5,   
   $��1��6   
�������$��0,5,0,5q�6/   .+�5-      �� -   5         ��Name$ ErrorRegion��5-   0    �� Orientation$ Vertical=�6   $��Center=�6E   $��Center=�6b   .+�5.      �� -   5        $	 ErrorMessage��5.      $��0,0,0,40q�62   $
��Wrap=�6D   �� 5/      $=��7Maybe You can solve this problem by reloading NightCity��6   $��Center=�6[   $��Center=�6x   ���#7575756�   �� 50      -	   5        +  %MaterialDesignThemes.Wpf.ButtonAssist   CornerRadius$ 5��50      $	��150��6F   $��Reload NightCity��6R   $��10(�6m   $��5q�6{   $��5q�6�   ���#FFCA28   MaterialDesignRaisedButton#���   6�   
���   �$��Center=�6  $��Center=�6,  ��6�   �� *OpenCommand+6G  51      �� 52      -
   5         ��Name$ TemplateRegion��52      |�.  $y�	Collapsed=�6G   53        54   
   $��30��6   $��2��6*   .��55      �  56      �� 57      $
��auto �6'   �� 58      $
��auto �6'   �� 59      $��* �6'   �� 5:      $
��auto �6'   .+�5=      �� -   5        $ Official��5=   Q   $ 
Horizontal=�6   $��0��66   $��5q�6F   .+�5>        $
 Seal=�6.   $��20��6:   $��20��6F   $��Center=�6Q   $��Center=�6n   �� 5?      $��Official��6    $��5,0,0,0q�60   $��Center=�6A   $��Center=�6^   5@      �� 5A      $ 
Horizontal=�6   $��1��66   $��5q�6F   .+�5B        $ AccountStar=�6.   $��20��6A   $��20��6M   $��Center=�6X   $��Center=�6u   �� 5C      -   5        $	 Author��5C       $��5,0,0,0q�60   $��Center=�6A   $��Center=�6^   5D      �� 5E      -   5        $
	 Foot��5E      $��5q�6*   $��3��65   $��Center=�6E   $��Center=�6b   5F      5G      5H      5J      