﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.269
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UEEditor.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("tcp://localhost:889/RemotePriceProcessor")]
        public string PriceProcessorURL {
            get {
                return ((string)(this["PriceProcessorURL"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Уведомление%20о%20нераспознанных%20наименованиях%20для%20{0}%20({1})")]
        public string AboutNamesSubject {
            get {
                return ((string)(this["AboutNamesSubject"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Уведомление%20о%20нераспознанных%20изготовителях%20для%20{0}%20({1})")]
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
%20%20%20%20%20Вы можете контролировать процесс обработки Вашего прайс-листа, используя Личный кабинет:%0D%0A
%20%20%20%20%20- раздел %22Формализация прайс-листов%22;%0D%0A
%20%20%20%20%20- раздел %22Сопоставление позиций из прайс-листов%22.%0D%0A%0D%0A")]
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
%20%20%20%20%20Вы можете контролировать процесс обработки Вашего прайс-листа, используя Личный кабинет:%0D%0A
%20%20%20%20%20- раздел %22Формализация прайс-листов%22;%0D%0A
%20%20%20%20%20- раздел %22Сопоставление позиций из прайс-листов%22.%0D%0A%0D%0A")]
        public string AboutNamesBody {
            get {
                return ((string)(this["AboutNamesBody"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("net.tcp://prg4:900/RemotePriceProcessorService")]
        public string WCFServiceUrl {
            get {
                return ((string)(this["WCFServiceUrl"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("KvasovTest@analit.net")]
        public string EmailService {
            get {
                return ((string)(this["EmailService"]));
            }
        }
    }
}
