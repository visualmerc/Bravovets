
(function ($) {
    $.extend($.ui, { SocialPostDialog: {} });

    function SocialPostDialog() {

        this._shareDialog = null;
        this._dialogType = null;
        this._fileUpload = null;
        this._url = name;
        this._scope = null;
        this._afterPost = null;
        this._afterDelete = null;
        this._origElement = null;
        this.widget = null;
    }

        $.extend(SocialPostDialog.prototype, {
            remove: function() {
                this.widget.unbind("click");
            },
            init: function(options, control) {
                this.widget = control;

                var defaults = {
                    "dialogType": "facebook",
                    "fileupload": true,
                    "afterPost": null
                };

                var allOptions = $.extend(defaults, options);
                this._dialogType = allOptions.dialogType;
                this._fileUpload = allOptions.fileupload;


                this._scope = this._dialogType;

                if (this._dialogType == "trendingtopics") {
                    this._url = "/sharesocial/getsyndicatedsocialpostdialog";
                } else if (this._dialogType == "socialcal") {
                    this._url = "/sharesocial/getqueuedsocialpostdialog";
                } else {
                    this._url = "/sharesocial/getsocialpostdialog";
                }

                this._afterPost = allOptions.afterPost;
                this._afterDelete = allOptions._afterDelete;
                this._bindShow(control);

                return this;
            },
            _bindShow: function(items) {
                var self = this;
                items.each(function(item) {
                    $(this).click(function(event) {
                        event.preventDefault();
                        self._show(this);
                    });
                });
            },
            _show: function(element) {
                var self = this;
                var id = "";
                var url = "";

                id = $(element).data("event-id");
                self._origElement = $(element);

                var data = {
                    "dialogType": self._dialogType,
                    "id": id,
                    "clientOffset": new Date().getTimezoneOffset()
                };

                $.ajax({
                    url: self._url,
                    data: data,
                    type: "GET",
                    cache: false,
                    success: function(results) {
                        self._showDialog(self, results);
                    }
                });
            },
            _showDialog: function(self, results) {

                var dialog = $(results);
                var type = "";

                if ($(dialog).hasClass("twitter")) {
                    type = "twitter";
                } else if ($(dialog).hasClass("facebook")) {
                    type = "facebook";
                } else {
                    type = self._dialogType;
                }

                self._shareDialog = dialog.modal()
                    .show(function() {
                        var shown = $(dialog).data('shown');
                        if (shown === true) {
                            return;
                        }
                        $(dialog).data('shown', true);

                        if (self._dialogType != "") {
                            self._dialogType = type;
                        }

                        self._bindShareDialog(self);
                    })
                    .on('hidden.bs.modal', function(e) {

                        var didPost = self._shareDialog.data("post");

                        if (didPost !== true) {
                            self._shareDialog_deleteQueuedItems(self);
                        }
                        self._shareDialog.remove();
                    });

            },
            _shareDialog_deleteQueuedItems: function(self) {
                if (self._scope == "socialcal") {
                    return;
                }

                self._deleteQueuedItem(self);
            },
            _deleteQueuedItem: function(self) {

                var queueContentId = $("#queuedContentId").val();

                if (!queueContentId || queueContentId === "") {
                    return;
                }

                var url = "/sharesocial/DeleteQueuedContent?queuedContentId=" + queueContentId;
                $.ajax({
                    url: url,
                    type: "DELETE",
                    success: function(data) {

                    }
                });
            },
            _totalAttachedMedia: function() {
                var count = $(".post-image-wrapper").length;
                return count;
            },
            _shareDialog_CheckMediaTypes: function(self) {

                if ($(".attach-media-form").is(":visible")) {
                    $(".attach-media-form").hide();
                    return;
                }

                var count = self._totalAttachedMedia();
                if (count > 1) {
                    $("#attachUrlVideo").addClass("disabled");
                } else {
                    $("#attachUrlVideo").removeClass("disabled");
                }
                $(".attach-media-form").show();

            },
            _shareDialog_InitDatePicker: function(self) {
                $('.datepicker-container').DatePicker({});


            },
            _shareDialog_InitTimePicker: function(self) {
                $('.timepicker-container').TimePicker();

                $("#postTime").change(function() {
                    $("#error-message-wrapper").hide();
                });

            },
            _shareDialog_DeletePost: function(self) {

                $("#deleteContent").click(function(event) {
                    event.preventDefault();
                    self._deleteQueuedItem(self);
                    self._shareDialog.data('post', true);
                    self._shareDialog.modal('hide');
                    $(self._origElement).remove();
                });
            },
            _shareDialog_InitPostNow: function(self) {
                var postnow = $("#postnow");
                var postTimeDiv = $('#postTime').parents('.form-group:first');
                var postDateDiv = $('#postDate').parents('.form-group:first');

                self._shareDialog_InitDatePicker(self);
                self._shareDialog_InitTimePicker(self);

                postnow.change(function() {

                    var isChecked = $(this).is(":checked");

                    if (isChecked == true) {
                        var postTimeVal = $("#postTime").val();

                        $("#postTime").data("orig", postTimeVal);
                        $("#postTime").val('');
                        $("#postTime").attr("disabled", '');
                        postTimeDiv.addClass('disabled');

                        var postDateVal = $("#postDate").val();

                        $("#postDate").data("orig", postDateVal);
                        $("#postDate").val('');
                        $("#postDate").attr("disabled", '');
                        postDateDiv.addClass('disabled');

                    } else {
                        var postTimeOrig = $("#postTime").data('orig');
                        if (!postTimeOrig) {
                            return;
                        }
                        $("#postTime").val(postTimeOrig).removeAttr("disabled");
                        postTimeDiv.removeClass('disabled');

                        var postDateOrig = $("#postDate").data('orig');
                        if (!postDateOrig) {
                            return;
                        }
                        $("#postDate").val(postDateOrig).removeAttr("disabled");
                        postDateDiv.removeClass('disabled');
                    }
                });

                postnow.trigger("change");
            },
        _isPostNow: function () {
            var postnow = $("#postnow");

            if (postnow.is(":checked")) {
                return true;
            } else {
                return false;
            }
        },
        _shareDialog_InitAttachMedia: function (self) {
            $("#attach-media").click(function (event) {
                self._shareDialog_CheckMediaTypes(self);
            });

            if ($("#urlAttachment").length > 0) {
                $("#attach-media").parent().hide();
            }


            $("#attachUrlVideo").click(function (event) {
                event.preventDefault();
                var url = $("#videoOrUrl").val();
                if (url == '') {
                    return;
                }

                $.ajax({
                    url: "/sharesocial/attachurl?queuedContentId=" + $("#queuedContentId").val() + "&url=" + url,
                    type: "POST",
                    success: function () {
                        $(".attach-media-form").hide();
                        $("#attach-media").parent().hide();
                        var data = {
                            "url": url
                        };
                        var result = tmpl("attach-url-template", data);
                        var newItem = $(result).appendTo($("#filesContainer"));
                        $("#videoOrUrl").val('');
                        $(".tweetlength-wrapper").TweetLength();
                        $(newItem).find("#delete-url-attachment").click(function (event) {
                            event.preventDefault();

                            $.ajax({
                                url: "/sharesocial/deleteurl?queuedContentId=" + $("#queuedContentId").val(),
                                type: "POST",
                                success: function () {
                                    $("#urlAttachment").remove();
                                    $(".attach-media-form").show();
                                    $("#attach-media").parent().show();
                                    $(".tweetlength-wrapper").TweetLength();
                                }
                            });

                        });
                    }
                });

            });
        },
        _shareDialog_InitFileUpload: function (self) {

            if (self._dialogType == "twitter" && $(".delete-attachment").length > 0) {
                $("#attach-media").parent().hide();
                $(".tweetlength-wrapper").TweetLength();
            }

            $(".delete-attachment").click(function () {
                var url = $(this).data("url");
                var attachmentId = $(this).data("attachmentid");
                self._deleteAttachment(url, attachmentId);
            });

            var maxFileSize = 26214400;
            var acceptFileTypes = "/(\.|\/)(gif|jpe?g|png|tiff|bmp)$/i";
            
            if (self._dialogType == "twitter") {
                maxFileSize = 3145728;
                acceptFileTypes = "/(\.|\/)(gif|jpe?g|png)$/i";
            }

            $('#fileupload').fileupload({
                url: '/sharesocial/uploadfile',
                acceptFileTypes: acceptFileTypes,
                maxFileSize: maxFileSize,
                filesContainer: "#filesContainer",
                dataType: 'json',
                autoUpload: true,
                always: function (e, data) {
                    $(".attach-media-form").hide();
                    if (self._dialogType == "twitter") {
                        $("#attach-media").parent().hide();
                        $(".tweetlength-wrapper").TweetLength();
                    }
                    var attachmentId = data.result.files[0].attachmentId;
                    var deleteIcon = $("#delete-" + attachmentId);
                    var url = deleteIcon.data("url");
                    $("#delete-" + attachmentId).click(function () {
                        self._deleteAttachment(url, attachmentId);
                    });
                }
            });
        },
        _shareDialog_InitPost: function (self) {
            $("#postContent").click(function (e) {

                if (self._validateMessage(self) == false) {
                    return;
                }

                var postTime = $("#postTime").val();
                var postDate = $("#postDate").val();

                var postNow = self._isPostNow();
                var url = null;
                var data = null;


                if (self._scope == "trendingtopics") {
                    var postToTwitter = $("#post-to-twitter").is(":checked");
                    var postToFacebook = $("#post-to-facebook").is(":checked");

                    data = {
                        "contentId": $("#syndicatedContentId").val(),
                        "message": $("#share-message").val(),
                        "postToTwitter": postToTwitter,
                        "postToFacebook": postToFacebook,
                        "postNow": postNow,
                        "postDate": postDate + " " + postTime,
                        "clientOffset": new Date().getTimezoneOffset()
                    };
                    url = "/sharesocial/queuesyndicatedcontent";
                } else {
                    if ($("#isPublished").val() == "true") {
                        url = "/sharesocial/clonequeuedcontent";
                    } else {
                        url = "/sharesocial/updatequeuedcontent";
                    }
                    data = {
                        "contentId": $("#queuedContentId").val(),
                        "message": $("#share-message").val(),
                        "postNow": postNow,
                        "postDate": postDate + " " + postTime,
                        "clientOffset": new Date().getTimezoneOffset()
                    };
                }
                $.ajax({
                    url: url,
                    data: data,
                    type: "POST",
                    success: function () {
                        self._shareDialog.data('post', true);
                        self._shareDialog.modal('hide');
                        if (self._afterPost != null) {
                            self._afterPost(self._origElement);
                        }
                    }
                });

            });
        },
        _validateMessage: function (self) {
            var postToTwitter = $("#post-to-twitter").is(":checked");
            var postToFacebook = $("#post-to-facebook").is(":checked");
            var errorMessage = $("#error-message");
            var errorMessageWrapper = $("#error-message-wrapper");
            var message = $("#share-message").val();
            var totalAttachedMedia = self._totalAttachedMedia();

            if (self._isPostNow() == false) {
                var date = new Date($("#postDate").val());

                var currentDate = new Date();
            
                if (date.toDateString() == currentDate.toDateString()) {

                    var time = $("#postTime").val();

                    var hour = time.replace(":00", "");
                    if (hour < currentDate.getHours()) {
                        errorMessage.text(errorMessage.data('futuredate'));
                        errorMessageWrapper.show();
                        return false;
                    }

                }
            
            }

            if (self._dialogType == "trendingtopics") {
                if (postToTwitter == false && postToFacebook == false) {
                    errorMessage.text(errorMessage.data('selectnetwork'));
                    errorMessageWrapper.show();
                    return false;
                }

                if (postToTwitter != false) {
                    var tweetLength = $(".tweetlength-wrapper");
                    if (tweetLength.hasClass("error")) {
                        return false;
                    }
                }

                if (message.length == 0) {
                    errorMessage.text(errorMessage.data('nomessage'));
                    errorMessageWrapper.show();
                    return false;
                }
            } else {
                if (message.length == 0 && totalAttachedMedia <= 1) {
                    errorMessage.text(errorMessage.data('nomessage'));
                    errorMessageWrapper.show();
                    return false;
                }
            }
            return true;
        },
        _shareDialog_InitShareOptions: function (self) {

            if (self._dialogType == "facebook") {
                $(".tweetlength-wrapper").hide();
            } else if (self._dialogType == "twitter") {
                $(".tweetlength-wrapper").show();
                $(".tweetlength-wrapper").TweetLength();
            } else {


                var postToTwitter = $("#post-to-twitter");
                postToTwitter.change(function () {
                    if ($(this).is(":checked")) {
                        $(".tweetlength-wrapper").show().TweetLength();
                        $("#error-message-wrapper").hide();
                    } else {
                        $(".tweetlength-wrapper").hide();
                    }
                });
                $(postToTwitter).trigger("change");

                var postToFacebook = $("#post-to-facebook");
                postToFacebook.change(function () {
                    if ($(this).is(":checked")) {
                        $("#error-message-wrapper").hide();
                    } else {

                    }
                });


            }
            $("#share-message").keydown(function () {
                if ($("#error-message-wrapper").is(":visible")) {
                    $("#error-message-wrapper").hide();
                }
            });
        },
        _bindShareDialog: function (self) {
            self._shareDialog_InitShareOptions(self);
            self._shareDialog_InitPostNow(self);
            if (self._fileUpload == true) {
                self._shareDialog_InitAttachMedia(self);
                self._shareDialog_InitFileUpload(self);
            }
            self._shareDialog_InitPost(self);
            self._shareDialog_DeletePost(self);
        },
        _deleteAttachment: function (url, id) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    $("#" + id).remove();
                    $("#attach-media").parent().show();
                    $(".tweetlength-wrapper").TweetLength();
                }
            });
        },
    });
    $.fn.socialPostDialog = function (options) {

        var control = new SocialPostDialog();
        control.init(options, this);
        return control;
    };
})(jQuery);