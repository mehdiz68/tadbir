
var language = navigator.userLanguage;
var tinyMCEpath = "/Scripts/tinymce/";
tinymce.init({
    branding: false,
    fontsize_formats: "10px 11px 12px 13px 14px 16px 18px 20px 22px 24px 26px 28px 30px 32px 34px 36px 38px 40px 42px 44px 46px 50px 60px 80px 100px",
    forced_root_block: 'p',
    force_br_newlines: false,
    force_p_newlines: true,
    mode: "textareas",
    theme: "modern",
    elements: "elm1",
    editor_selector: "elm1",
    relative_urls: false,
    remove_script_host: false,
    convert_urls: true,
    height: 420,
    directionality: "rtl",
    content_css: [tinyMCEpath + "tiny_rtl.css"], font_formats: 'IRANSansWeb=IRANSans;Tahoma=Tahoma;Arial=Arial;Yekan=Yekan',
    width: 755,
    height: 500,
    resize: false,
    autosave_ask_before_unload: true,
    codesample_dialog_width: 600,
    codesample_dialog_height: 425,
    template_popup_width: 600,
    template_popup_height: 450,
    powerpaste_allow_local_images: true,
    plugins: [
      " advlist anchor autolink codesample colorpicker contextmenu fullscreen help image imagetools directionality",
      " lists link media noneditable preview",
      " searchreplace table textcolor visualblocks wordcount code"
    ], //removed:  charmap insertdatetime print
    toolbar:
      " undo redo | bold italic | forecolor backcolor | fontselect fontsizeselect |  codesample | alignleft aligncenter alignright alignjustify | outdent indent blockquote | bullist numlist | media image | link unlink anchor | removeformat preview fullscreen | ltr rtl | code",
    mobile: {
        theme: 'mobile'
    }
});
//tinyMCE.init({
//    branding: false,

//    theme_advanced_font_sizes: "10px,11px,12px,13px,14px,16px,18px,20px,22px,24px,26px,28px,30px,32px,34px,36px,38px,40px,,46px,50px",
//    theme_advanced_fonts: "Yekan,Tahoma, Arial ,Trebuchet MS',Verdana, Helvetica, sans-serif  ",

//    forced_root_block: 'p',
//    force_br_newlines: false,
//    force_p_newlines: true,
//    mode: "textareas",
//    theme: "modern",
//    elements: "elm1",
//    editor_selector: "elm1",
//    relative_urls: false,
//    remove_script_host: false,
//    convert_urls: true,
//    height: 420,
//    directionality: "rtl",
//    content_css: tinyMCEpath+"tiny_rtl.css",
//    plugins: "autolink,lists,pagebreak,style,layer,table,save,advhr,advimage,advlink,insertdatetime,preview,media,searchreplace,contextmenu,paste,directionality,fullscreen,xhtmlxtras,template,wordcount,advlist",
//    theme_advanced_buttons1: "template,bold,italic,underline,strikethrough,|,justifyleft,justifycenter,justifyright,justifyfull,styleselect,formatselect,fontselect,fontsizeselect,|,replace,|,bullist,numlist,|,outdent,indent,blockquote",
//    theme_advanced_buttons2: "link,unlink,anchor,help,code,|,insertdate,inserttime,|,forecolor,backcolor,|,tablecontrols",
//    theme_advanced_buttons3: "hr,removeformat,visualaid,|,sub,sup,|,charmap,|,ltr,rtl,|,pasteword,pastetext,preview,fullscreen",
//    theme_advanced_toolbar_location: "top",
//    theme_advanced_toolbar_align: "left",
//    theme_advanced_statusbar_location: "bottom",
//    template_external_list_url: tinyMCEpath+"template_list.js",
//    extended_valid_elements: "script[src|type]",
//    extended_valid_elements: "img[src|lang|alt|align|class|style],iframe[width|height|frameborder|scrolling|marginheight|marginwidth|src|style]"

//});


function LoadEditor() {
    var language = navigator.userLanguage;
    var tinyMCEpath = "/Scripts/tinymce/";
    tinyMCE.init({
        branding: false,
        fontsize_formats: "10px 11px 12px 13px 14px 16px 18px 20px 22px 24px 26px 28px 30px 32px 34px 36px 38px 40px 42px 44px 46px 50px 60px 80px 100px",
        forced_root_block: 'p',
        force_br_newlines: false,
        force_p_newlines: true,
        mode: "textareas",
        theme: "modern",
        elements: "elm1",
        editor_selector: "elm1",
        relative_urls: false,
        remove_script_host: false,
        convert_urls: true,
        height: 420,
        directionality: "rtl",
        content_css: [tinyMCEpath + "tiny_rtl.css"], font_formats: 'IRANSansWeb=IRANSans;Tahoma=Tahoma;Arial=Arial;Yekan=Yekan',
        width: 755,
        height: 500,
        resize: false,
        autosave_ask_before_unload: false,
        codesample_dialog_width: 600,
        codesample_dialog_height: 425,
        template_popup_width: 600,
        template_popup_height: 450,
        powerpaste_allow_local_images: true,
        plugins: [
          " advlist anchor autolink codesample colorpicker contextmenu fullscreen help image imagetools directionality",
          " lists link media noneditable preview",
          " searchreplace table textcolor visualblocks wordcount code"
        ], //removed:  charmap insertdatetime print
        toolbar:
          " undo redo | bold italic | forecolor backcolor | fontselect fontsizeselect |  codesample | alignleft aligncenter alignright alignjustify | outdent indent blockquote | bullist numlist | media image | link unlink anchor | removeformat preview fullscreen | ltr rtl | code",
        mobile: {
            theme: 'mobile'
        }

    });
}