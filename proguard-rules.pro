# �������� ����� �������
-keep public class * extends android.app.Activity
-keep public class * extends android.app.Application

# �������� ������� �������� ���������
-keep class com.google.android.gms.** { *; }
-keep class com.android.billingclient.** { *; }