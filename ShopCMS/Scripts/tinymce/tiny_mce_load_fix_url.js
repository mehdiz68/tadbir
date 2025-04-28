
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