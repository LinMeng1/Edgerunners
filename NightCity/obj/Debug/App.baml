   M S B A M L   `   `   `  ����D  @NightCity, Version=1.0.0.7, Culture=neutral, PublicKeyToken=null%clr-namespace:NightCity	NightCity  5          NightCity.App   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      )localclr-namespace:NightCity     5      _ [MaterialDesignThemes.Wpf, Version=4.9.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5      T PPrism.Wpf, Version=8.1.97.5141, Culture=neutral, PublicKeyToken=40ee6c3a2184dc59[ WPrism.DryIoc.Wpf, Version=8.1.97.5141, Culture=neutral, PublicKeyToken=40ee6c3a2184dc59&prismhttp://prismlibrary.com/   5        �� 	Resources  5       �� MergedDictionaries+  %MaterialDesignThemes.Wpf.BundledTheme   	BaseTheme   PrimaryColor   SecondaryColor �� Source   Key  ScrollThumbs �� 
TargetType �� Property �� Value	 �� 
TargetType
 �Name ��Name �� CornerRadius �� Triggers P� Property P� Value �� 
TargetName � RowDefinitions ]�Name ]� Thumb n�Name ]� IncreaseRepeatButton ��Name J� Opacity ]� DecreaseRepeatButton P� 
SourceName  ButtonSelectBrush  	DarkBrush �� Angle�� 5   
    5	        5
      $ LightX�6.   $ 
DeepPurpleX�6@   $ PurpleX�6Z   �� 5      $h bpack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Defaults.xaml?�6%   5      �� 5      *�5      Poppins  MIpack://application:,,,/Resources/Font/Poppins-Regular.ttf#Poppins Regular)�6|   5      �� 5      $. (Resources/DefaultSettingsDictionary.xaml?�6%   5      %�  &	       '�� C    �� 5      " n�6)   .�5      ��  � �� 5      �� 5      "	 n�6*   .R�5      � $

 Grid��6#   .+�5   "   �� $��Stretch=�6,   $��Stretch=�6J   $
��Auto��6f   $
��Auto��6s   
������ 6�   �� 5   "   $ 
Rectangle1��6)   $ 5��6=   $��Stretch=�6N   $��Stretch=�6l   $
��Auto��6�   $
��Auto��6�   #��z��6�   5       5      P� 5   "    �@�Tag$ 
Horizontal��69   . �5   &   �� $ 
Rectangle1��6-    � ��$
 Auto��6V   �� 5   &   $ 
Rectangle1��6-    � ��$ 7��6W   5    #   5"      5$      5%      �� 5'      " ��6/   .�5(      ��  �@��IsFlicksEnabled .  6;   �� 5)       � �� �#8C8C8C6/   �� 5*       � ��
 ���� 6/   �� 5+       � ��$ 8��6*   �� 5,       � �� 5-      �� 5.      "	 ��6*   .R�5/      � $
 GridRoot��6#   $��8��65   #��z��6?    50   "   �� 51   &   $��0.00001* �64   .+�53   "   ]� $ 
PART_Track��6(   $��0��6<   ��. |�.   54   &   n� 55   *   $ Thumb��60   #��z��6?   #���  6i    57   &   �� 58   *   $ PageUp��67   $��ScrollBar.PageDownCommand��6G   $ 0Q�6k   |�.  6w    5:   &   �� 5;   *   $ PageDown��67   $��ScrollBar.PageUpCommand��6I   $ 0Q�6k   |�.  6w   5=   #   5>       5?      P� 5@   "   $ Thumb��6*    �@J�IsMouseOver . . �5A   &   �� # �  6-   $ Thumb��6Y    � ��6l   5B   #   P� 5C   "   $ Thumb��6*    �@n�
IsDragging . . �5D   &   �� # �  6-   $ Thumb��6Q    � ��6d   5E   #   P� 5F   "    � {� .  . �5G   &   �� $ Thumb��6-    � y�$ 	Collapsed=�6V   5H   #   P� 5I   "    �@��Orientation$ 
Horizontal=�6A   . �5J   &   �� $ GridRoot��6-    �@�LayoutTransform 5K   *   �� 5L   .   $	 -90Q�6>   5N   '   �� 5O   &   $ 
PART_Track��6-    �@�LayoutTransform 5P   *   �� 5Q   .   $	 -90Q�6>   5S   '   �� 5T   &    � ��$
 Auto��6>   �� 5U   &    � ��$ 8��6?   �� 5V   &   $ Thumb��6-    �@�Tag$ 
Horizontal��6O   �� 5W   &   $ PageDown��6-    � ��$ ScrollBar.PageLeftCommand��6V   �� 5X   &   $ PageUp��6-    � ��$  ScrollBar.PageRightCommand��6T   5Y   #   5[      5]      5^      5_      5a      