   M S B A M L   `   `   `  ���� R  NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35L5clr-namespace:System.Diagnostics;assembly=WindowsBaseSystem.Diagnostics  C ?RingRing, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:RingRingRingRing    RingRing.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml      @d2http://schemas.microsoft.com/expression/blend/2008      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006      Fdiag5clr-namespace:System.Diagnostics;assembly=WindowsBase      (localclr-namespace:RingRing        � Title$  
MainWindow�� � WindowStartupLocation$ Manual=� � 
ResizeMode$ NoResize=�$	��720��$	��500��$	��500��$	��500��$	��720��$	��720��
������  � WindowStyle$
 None=� � AllowsTransparency .  � 	Resources  �� 
TargetType �� 	Resources �� Color   ScrollBarThumb	 �� Property
 �� Value �� 
TargetType �� CornerRadius  VerticalScrollBar � RowDefinitions ]� Thumb �� Triggers P� Property P� Value 
 toggle  ContainerStyle 9� 
IsExpanded J� Opacity �� Left
 �� Top J� OpacityMask  ContainerStyleTxn �� Mode �� StringFormat  ImageButtonStyle��%�  '��       &	  R     &	 [    7   '�� �    7  &	 �    &	 �    &	     &	 @    �� " �� ���� $ #F9F6F6��(��       *SystemColors.ControlBrushKey+)�� " n�.��� 	 �@K�ZIndex$
 1���� 	 �@J�SnapsToDevicePixels
 . �� 	 �@�OverridesDefaultStyle
 . �� 	 � ��
 . �� 	 � |�
 . �� 	 �@J�Opacity$
 1Q��� 	 � ��
 �� " n�.R��� $��Border��$ 5�����	#FFE7E4E6�� " ��.R��  �� $��* �.+��� $��1��
�������$��1,0,0,0q�
��� ���$ 5��]� $��
PART_Track��$��1����.  n� 8��  $��1,1,1,1q��� " ��.��� 	 �@J�SnapsToDevicePixels
 . �� 	 �@�OverridesDefaultStyle
 .  P�  �@��Orientation$ Vertical=�. ��� 	 � ��$
 12���� 	 � ��$	
 250���� 	 � ��8
   �� " d�.��� 	 � ��

 � � ��� " ��.��� 	 � ��
 �� .R�9� $��expand�� .  $��RightToLeft=�$��	0,0,20,10q����� $��LeftToRight=�
��� � �.+��� $��BorderPanel��}�. 
������ $��50��$	��380��$��0,2,10,1q�._��� .+��� $!��12:55 PM February 26th 2018��$
��Left=�$	��150��$
��None=�$��SemiBold"�$��10(����#9e9e9e$	 0.8Q�$	 7.5��$ 8�� Z� �� $
��Left=�$	��100��$
��None=�$
��Bold"�$��18(����#3c4788$	 0.8Q�$
 24.5��$ 6������ *Name+ Z� .���� $��0,10,0,0q��� 	 � ��

 ������� " ��.��� 	 � ��
 �� .R�9� $��expand�� .  $��RightToLeft=�$��	0,0,20,10q����� $��LeftToRight=�.+��� $��BorderPanel��}�. 
������ $��50��$	��380��$��0,2,10,1q�._��� .+��� $
��Left=�$	��150��$
��None=�$��SemiBold"�$��10(����#9e9e9e$	 0.8Q�$	 7.5��$ 8������ *Name+ Z� �� $
��Left=�$	��100��$
��None=�$
��Bold"�$��18(����#3c4788$	 0.8Q�$
 24.5��$ 6������ *Items[0].OrderAmount+$ OneWay=�$ Rs. {0}�� Z� .���� $��0,0,0,0q��� 	 � ��

 ������� " ��.��� 	 � ��
 �� " ��.R��� .��� -   $��MainForm�� �� $
��187* ��� $	��53* �.+��� -   $��BorderTransactionPanel��$y�Hidden=�}�. $ 15�����#fafafa$	��480��$	��460��$��10,190,10,70q�$��2��._��� -   $��canvasTransaction��
������ .+��� -    ��Name$ _lblStoreName��$��Store Name Here��$
��Left=�$	��Top=�$��244.993��$��20(�$
��Bold"����	#FF3C4788$ 35��$ 44���� -   $ _lblDashLine��$
��Left=�$��34��$	��Top=�$��436.5�����#c3c3c3$��	ExtraBold"�$����________________________________________________________________________________________________________________________________________________________________________________��$ 32��$ 72���� -   $ 	_lblTvsId��$��TVS ID: ��$
��Left=�$	��Top=�$��244.993�����	#FFA0A097$��16(�$
��Bold"�$ 34��$ 24���� -   $ 
_lblHeader��$
��Left=�$	��Top=�$	��425�����	#FF0C0C00$��	ExtraBold"�$ 34��$ 100.25��$	 0.7Q��� -    ��Name$ _btnCalender����.  #��[ $��0q�
������ $ 40��$	 280��$��30��$	��100��.���� -	   $��_btnCalenderImage��$"��/Resources/todayCalender.png��$��0,0,0,0q�$��30���� -
   $��lvItems����.  $��Visible=�
������ $��Disabled=�$��0q����	#FFABADB3$ 32��$ 129.5��$	��250��$	��444��$	��460������ .R��� $��BorderPanel��-   }�. $	��0.5q�$ 5��$��40��$	��410��$��1,3,0,1q����� *BorderColor+1   +System.Diagnostics.PresentationTraceSources   
TraceLevel$
 High=����� *BackgroundColor+._��� $��innercanvas��.+��� -   $	��230��$
��None=�$
��Bold"�$��14(�$��	10,10,0,0q����	#FF9E9E9E$	 0.8Q����� *ProductName+ Z� �� $
��Bold"�$��14(�$��
290,10,0,0q���`� $  {0} {1}��.1���  �� Path$ Currency��� $ Amount��� $��
_okImage11��$��28��$��28��$��	370,4,0,0q����� *Image+ ��%�   (��       * SystemColors.HighlightBrushKey+)(��      *$"SystemColors.HighlightTextBrushKey+)�� $ Transparent���� $ Black������ " �� P�  �@J�IsMouseOver . . ��� 	 � ��

 �   ��� 	 � ��

 ���� �� 	 � ��
 ��#00FF00��]�  ]� 
Conditions ��  �� Property �@��
IsSelected �� Value . ��  �@��IsSelectionActive . �� -   $��lvTxnHistory��  �� SelectionMode$  Extended=�$y�Hidden=�
������ $	��250��$	��444��$	��460��$��Visible=�$��Hidden=�$ 32��$ 129.5��$��0q����	#FFABADB3��.  $��1������ .R��� $  Single=�$y�Visible=�
������ $
��auto��$	��410��$	��410��$��Hidden=�$��Hidden=�$��0q����	#FFABADB3��. ���� *
products+���� .R�� $��LeftToRight=�.+��� $��BorderPanel��}�. $��1q�
�������$ 5��
�������$��40��$	��400��$��0,2,0,3q�._��� .+��� $	��280��$
��None=�$��DemiBold"�$��12(�$��
10,10,10,0q����	#FF9E9E9E$ 1Q����� *ProductName+ Z� �� ! �� TextAlignment$! Right=�$��80��$
��Bold"�$��12(�$��
300,10,5,0q����#4a4a4a���� *Amount+$ OneWay=�$ Rs. {0}��" �� 
GroupStyle" (  "System.Windows.Controls.GroupStyle  #   ContainerStyle## [ "   ## [  ��%�   (��       * SystemColors.HighlightBrushKey+)(��      *$"SystemColors.HighlightTextBrushKey+)�� $ Transparent���� $ Black������ " �� P�  �@��
IsSelected . . ��� 	 � ��
 Z� �� 	 � ��
 Z� �� -   $��BorderFooterPanel��$y�Visible=�}�. $ 	0,0,15,15��
�������$��80��$	��480��$��0,380,10,70q�$��1��._��� -   $��canvasFooter��.+��� -   $ _lblTotalDiscountAmount��$��Rs. 0.00��$��174.993�����#4a4a4a$
��Bold"�$��40.67��
 �   �$ 25.33��$
 20.5��$
��Left=�$��Bottom=�$��27(��� -   $ _lblTotalDiscount��$��Total discount:��$
��Left=�$	��Top=�$��112.993�����	#FFA0A097$��11(�$
��Bold"�$��23.33��$
 20.5��$ 12���� -   $ _btnSave��$y�Visible=�#��[ $��0q�
������ $ 25��$	 250��$	��200��.���� -   $��_btnSaveImage��$��/Resources/save_Green.png��$��0,0,0,0q�$	��Top=�$��40��$	��200���� -   $��BorderFooterHistoryPanel��$y�Hidden=�}�. $ 	0,0,15,15��
�������$��80��$	��480��$��0,380,10,70q�$��1��._��� -   $��canvasFooterHistory��.+��� -   $$ _lblTotalDiscountAmountHistory��$��Rs. 0.00��$��174.993�����#4a4a4a$
��Bold"�$��40.67��
 �   �$ 25.33��$
 20.5��$
��Left=�$��Bottom=�$��27(��� -   $ _lblTotalDiscountHistory��$��Daily total:��$
��Left=�$	��Top=�$��112.993�����	#FFA0A097$��11(�$
��Bold"�$��23.33��$
 20.5��$ 12���� -   $ _btnSaveHistory��#��[ $��0q�
������ $ 25��$	 250��$	��200��.���� -   $��_btnSaveImageHistory��$��/Resources/exportCsv.png��$��0,0,0,0q�$	��Top=�$��40��$	��200���� -   $��_bubbleImage��$#��/Resources/bubble-shape3x.png��$	��212��$!��339.776,4.444,5.224,344.556q��� -   $��canvasEnterPin��$y�Visible=�
������ $!��355.77,27.555,23.23,412.746q�.+��� -   $ _lbl_lastTxn��$y�Hidden=�
$ � Tag$$ Last transaction��$��70��$��20��$��Center=�$ 25.332��$ 99.52��.���� % � Cursor$
% Hand��$��	Underline{����	#FF3C4788$��8(�$
��Bold"����� $	 Tag�& �� RelativeSource& �� ' �� Mode$' FindAncestor=�( �� AncestorType"( ���� -   $��_okImage��$��34��$��34��$��/Resources/ok_Green.png��$��0,0,0,0q�$y�Hidden=�) J� RenderTransformOrigin$) 0.177,-4.308&��� -   $ _lbl_Pin��$��	Enter PIN�����	#FF3C4788$��14(�$��SemiBold"�$��Center=�$��Center=�$��95.668��$��Stretch=�$ 14.332��$ 2���� -   $��	_enterPin��$��/Resources/enterPin.png��$��61��$	��100��$ 35.62��$ 10���� -    * ��Name$* 
textBoxOtp��$
��Left=�$��61��$��22(�+ �� TextWrapping$
+ Wrap=�$�� ��, �� TextAlignment$, Center=�$	��100��$��0��$��0q�- �� 	MaxLength$- 20��. �� MaxLines$. 1��
�������$��Stretch=�$	��Top=�$ 10��$ 35.62����Z� .���� -!   $��canvasCoupanDisplay��$y�Hidden=�
������ $!��355.77,27.555,23.23,412.746q�.+��� -"   $ _lbl_coupan�����#9e9e9e$��0��/ �� ContentStringFormat$/  {0} coupon(s)��$��95.668��$��34.158��$��Center=�$ 13.332��$ 5.342���� -#   $ _lbl_amount�����#4a4a4a$��20(�$
��Bold"�$��Center=�$��Center=�$��95.668��$��Stretch=�$ 13.332��$ 26������ 0 �� ElementName$0 _lblTotalDiscountAmount��$ Content��� -$   $ _lbl_editorder��$$ 
Edit order��$��70��$��20��$��Center=�$ 28.332��$ 96.52��.���� -%   $
% Hand��$��	Underline{����	#FF3C4788$��8(�$
��Bold"����� $	 Tag�& �� $' FindAncestor=�"( ���� -&   $ 
_btnRedeem��#��[ $��0q�
������ $ 7��$ 65��.���� -'   $��_btnRedeemImage��$!��/Resources/redeem_Green.png��$��0,0,0,0q�$	��Top=�$
��Left=�$	��108��$��25���� -(   $��canvasFinishTxn��$y�Hidden=�
������ $!��355.77,27.555,23.23,412.746q�.+��� -)   $ _lbl_FinishTotalDiscount�����#9e9e9e$��Total discount��$��95.668��$��27.908��$��Center=�$ 13.332��$ 5.342���� -*   $ _lbl_FinishTotalAmount�����#4a4a4a$��20(�$
��Bold"�$��Center=�$��Center=�$��95.668��$��Stretch=�$ 13.332��$ 26������ $0 _lblTotalDiscountAmount��$ Content��� -+   $ _btnFinishTxn��$
% Hand��#��[ $��0q�
������ $ 7��$ 65��.���� -,   $��_btnFinishTxnImage��$(��"/Resources/FinishandCopy_Green.png��$��0,0,0,0q�$	��Top=�$
��Left=�$	��108��$��25���� --   $��canvasUserInfo��$y�Hidden=�
������ $!��355.77,27.555,23.23,412.746q�.+��� -.   $��	_okImage1��$��28��$��28��$��/Resources/ok_Green.png��$��0,0,0,0q�$y�Visible=�$) 0.177,-4.308&�$
 47.5���� -/   $ _lbl_UserName�����#3c4788$��DemiBold"�$��15(�$��Mayank��$��95.668��$��34.158��$��Center=�$ 13.332��$
 32.5���� -0   $ _lbl_IsMember��$��is RingRing member!�����#9e9e9e$��10(�$��DemiBold"�$��Center=�$��Center=�$	��121��$��Stretch=�$ 51.25���� -1   $ _lbl_NextCustomer��$y�Hidden=�$$ Next Customer��$��80��$��26.25��$��Center=�$ 22��$ 90.27��.���� -2   $
% Hand��$��	Underline{����	#FF3C4788$��10(�$��DemiBold"����� $	 Tag�& �� $' FindAncestor=�"( ���� -3   $
��Back��$
% Hand��$��24��$��24��$��/Resources/Canceltxn.png��$��0,0,0,0q�$y�Visible=�$
 92.5��$
 11.5���� -4   $��Forward��$
% Hand��$��28��$��28��$!��/Resources/forwardImage.png��$��0,0,0,0q�$y�Visible=�$ 48��$ 83���� -5   $��canvasEditOrder��$y�Hidden=�
������ $!��355.77,27.555,23.23,412.746q�.+��� -6   1 ��Name$1 _lbl_UserName1��$
��Wrap=����#3c4788$��You're editing the order��$��95.668��$��36.658��$ 13.332��$
 32.5��$
��Bold"�$��14(�$! Center=��� -7   $��	BackImage��$
% Hand��$��28��$��28��$��/Resources/backImage.png��$��0,0,0,0q�$y�Visible=�$) 0.177,-4.308&�$
 47.5��$
 82.5���� -8   $��CloseTxn��$
% Hand��$��24��$��24��$��/Resources/CancelTxn.png��$��0,0,0,0q�$y�Visible=�$) 0.177,-4.308&�$
 92.5��$
 11.5���� -9   $��
_wifiImage��$��24��$��24��$��/Resources/wifi_Green.png��$��454,7,22,530.25q�