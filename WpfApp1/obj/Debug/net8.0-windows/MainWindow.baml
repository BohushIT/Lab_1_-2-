   M S B A M L   `   `   `  ����  WpfApp1    WpfApp1.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  
MainWindow��5   	   $	��200��6   $	��500��6)   .��5      �  � ColumnDefinitions 5   
   �� 5      $	��250 �6   �� 5      $	��250 �6    � RowDefinitions 5
   
   �� 5      $��50 �6   �� 5      $��50 �6   �� 5      $��50 �6   .+�5   
   �� -   5         ��Name$ CityTextBox��5      $��0��6   $	��200��64   $��25��6@   $��10q�6L   �� 5   
   -   5        $��1��5      $!��Get Weather (Without Redis)��6   $	��180��6E   $��30��6Q   5      �� 5   
   -   5        $��2��5      $��0��6   $��Get Weather (With Redis)��6.   $	��180��6Q   $��30��6]   5      �� 5   
   -   5         ��Name$ WeatherResultTextBlock��5      $��1��64   $
��Wrap=�6E   $	��200��6Y   $	��250��6e   $��25,23,25,-123q�6r   $��3��6�   5      5      