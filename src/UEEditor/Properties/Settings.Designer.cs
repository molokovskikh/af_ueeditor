﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3053
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UEEditor.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "9.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Уведомление%20о%20нераспознанных%20наименованиях%20для%20{0}")]
        public string AboutNamesSubject {
            get {
                return ((string)(this["AboutNamesSubject"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Уведомление%20о%20нераспознанных%20изготовителях%20для%20{0}")]
        public string AboutFirmSubject {
            get {
                return ((string)(this["AboutFirmSubject"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"Здравствуйте,%0D%0A%0D%0A
%20%20%20%20%20При обработке Вашего {0} прайс-листа не удалось сопоставить наименования некоторых изготовителей с нашим каталогом по причинам, описанным ниже:%0D%0A%0D%0A%0D%0A%0D%0A
%20%20%20%20%20Просьба внести изменения в написание указанных изготовителей в Вашем прайс-листе.%0D%0A
Уведомляем вас, что до внесения изменений или получения нами от Вас информации по этому вопросу, перечисленные изготовители не будут указываться в Вашем прайс-листе.%0D%0A%0D%0A
%20%20%20%20%20Вы можете контролировать процесс обработки Вашего прайс-листа, используя интерфейс управления:%0D%0A
%20%20%20%20%20- раздел ""Прайс-листы"" - ссылка ""Информация о формализации"";%0D%0A
%20%20%20%20%20- раздел ""Управление сопоставлением"".%0D%0A%0D%0A
С уважением,%0D%0A
Аналитическая компания ""Инфорум"" г.Воронеж%0D%0A
Тел.:%0D%0A
Москва +7 495 6628727%0D%0A
С.-Петербург +7 812 3090521%0D%0A
Воронеж +7 4732 606000%0D%0A
Челябинск +7 351 7298143%0D%0A
E-mail: pharm@analit.net%0D%0A
http://www.analit.net")]
        public string AboutFirmBody {
            get {
                return ((string)(this["AboutFirmBody"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"Здравствуйте,%0D%0A%0D%0A
%20%20%20%20%20При обработке Вашего {0} прайс-листа не опубликован (не доступен Вашим клиентам) ряд позиций, которые не удалось сопоставить с торговыми наименованиями нашего каталога по причинам, описанным ниже: %0D%0A%0D%0A%0D%0A%0D%0A
%20%20%20%20%20Просьба внести изменения в написание указанных позиций Вашего прайс-листа.%0D%0A
Уведомляем вас, что до внесения изменений или получения нами от Вас информации по этому вопросу, перечисленные позиции будут блокированы.%0D%0A%0D%0A
%20%20%20%20%20Вы можете контролировать процесс обработки Вашего прайс-листа, используя интерфейс управления:%0D%0A
%20%20%20%20%20- раздел ""Прайс-листы"" - ссылка ""Информация о формализации"";%0D%0A
%20%20%20%20%20- раздел ""Управление сопоставлением"".%0D%0A%0D%0A
С уважением,%0D%0A
Аналитическая компания ""Инфорум"" г.Воронеж%0D%0A
Тел.:%0D%0A
Москва +7 495 6628727%0D%0A
С.-Петербург +7 812 3090521%0D%0A
Воронеж +7 4732 606000%0D%0A
Челябинск +7 351 7298143%0D%0A
E-mail: pharm@analit.net%0D%0A
http://www.analit.net")]
        public string AboutNamesBody {
            get {
                return ((string)(this["AboutNamesBody"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("mail.adc.analit.net")]
        public string SMTPHost {
            get {
                return ((string)(this["SMTPHost"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\\\fmsold\\Prices\\")]
        public string RootPath {
            get {
                return ((string)(this["RootPath"]));
            }
        }
    }
}
