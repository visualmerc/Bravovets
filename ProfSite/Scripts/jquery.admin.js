
(function ($) {
    $.extend($.ui, { ManagePosts: {} });

    function ManagePosts() {
        this.widget = null;
        this.getUrl = null;
        this.pastNewestLabel = null;
        this.pastOldestLabel = null;
        this.futureNewestLabel = null;
        this.futureOldestLabel = null;
    }

    $.extend(ManagePosts.prototype, {
        init: function (options, control) {
            var self = this;
            this.widget = control;

            var defaults = {

            };

            var allOptions = $.extend(defaults, options);

            self.getUrl = allOptions.getUrl;

            self.pastNewestLabel = allOptions.pastNewestLabel;
            self.pastOldestLabel = allOptions.pastOldestLabel;
            self.futureNewestLabel = allOptions.futureNewestLabel;
            self.futureOldestLabel = allOptions.futureOldestLabel;

            self._bindEvents();
            self._getData(true);
            return this;
        },
        _bindEvents: function () {
            var self = this;
            $("#sortByOldest,#sortByNewest").click(function (event) {
                event.preventDefault();

                var text = $(this).text();
                $("#sortByLabel").text(text);

                $("#sortByItems>li>a.active").removeClass("active");
                $(this).addClass("active");
                self._getData(true);
            });
            self._bindTab();

        },
        _bindTab: function () {
            var self = this;

            self._setSortBy();
            

            $('.nav-tabs').bind('shown.bs.tab', function (e) {
                self._setSortBy();

                self._getData(true);
            });
        },
        _setSortBy: function () {
            var self = this;
            var activeTab = $(".tab-pane.active").attr("id");
            if (activeTab == "future") {
                $("#sortByLabel").text(self.futureNewestLabel);
                $("#sortByNewest").text(self.futureNewestLabel);
                $("#sortByOldest").text(self.futureOldestLabel);
            } else {
                $("#sortByLabel").text(self.pastNewestLabel);
                $("#sortByNewest").text(self.pastNewestLabel);
                $("#sortByOldest").text(self.pastOldestLabel);
            }
        },
        _getPageNumber: function (element) {
            var pageNumber = element.first().text() - 1;
            if (pageNumber < 0) {
                pageNumber = 0;
            }
            return pageNumber;
        },
        _refreshPaging: function () {
            var self = this;
            var active = $(".pagination>li.active");
            var anchor = active.find("a");


            var pageNumber = self._getPageNumber(anchor);
            $(".page-" + pageNumber).addClass("active");
            //TODO:If no pages then disable all buttons
            if (pageNumber == 0) {
                //first page
                $(".previous").attr("disabled", "true");
                $(".next").removeAttr("disabled");
                return;
            }
            $(".previous").removeAttr("disabled");

            var nextPage = active.next();
            if (nextPage.length == 0) {
                $(".next").attr("disabled", "true");
            } else {
                $(".next").removeAttr("disabled");
            }

        },
        _getData: function (refresh) {
            var self = this;
            var sortBy = $("#sortByItems>li>a.active").data("sort");
            var page = self._getPageNumber($(".pagination>li.active>a"));

            var activeTab = $(".tab-pane.active").attr("id");

            var data = "?type=" + activeTab + "&clientOffset=" + new Date().getTimezoneOffset() + "&sortBy={0}&page={1}".format(sortBy, page);

            var tbody = $("#" + activeTab + ">table>tbody");
            

            $("#sortBy").attr("disabled", "true");
            $(".pagination>li>a").attr("disabled", "true");
            $(".next").attr("disabled", "true");
            $(".previous").attr("disabled", "true");
            tbody.empty();
            $.ajax({
                url: self.getUrl + data,
                type: "GET",
                cache: false,
                success: function (data) {
                    //TODO:remove loading row
                    //TODO:enable paging
                    //TODO:enable sort
                    //TODO:enable tab
                    var results = $(data);
                    var items = results.find("tr");

                    var pagecount = results.data("pagecount");

                    tbody.append(items);
                    $("#sortBy").removeAttr("disabled", "true");
                    $(".pagination>li>a").removeAttr("disabled", "true");
                    if (refresh == true) {
                        self._createPaging(self, pagecount);
                    } else {
                        self._refreshPaging();
                    }
                    self._bindItemEvents();
                }
            });
        },
        _bindItemEvents: function () {
            var self = this;
            $(".delete-post").click(function (event) {
                event.preventDefault();

                var confirmDelteText = $("#adminManagePosts").data("confirmdelete");
                if (confirm(confirmDelteText) == false) {
                    return;
                }

                var id = $(this).data("id");
                var itemToDelete = $("#post-" + id);

                itemToDelete.fadeTo("slow", 90, function () {
                    $.ajax({
                        url: self.getUrl + "?id=" + id,
                        type: "DELETE",
                        success: function (data) {
                            itemToDelete.remove();
                        }
                    });

                });



            });


        },
        _createPaging: function (self, pagecount) {
            var pager = $(".pagination");
            $(pager).find("a").unbind("click");
            $(pager).html('');

            if (!pagecount || pagecount == 0) {
                $(".previous").hide();
                $(".next").hide();
                return;
            } else if (pagecount == 1) {
                $(".previous").hide();
                $(".next").hide();
                return;
            }
            $(".previous").show();
            $(".next").show();
            //Create pager
            for (var i = 0; i < pagecount; i++) {
                var active = "";
                if (i == 0) {
                    active = "active";
                };
                var item = $('<li class="' + active + ' page-' + i + '"><a  href="#">' + (i + 1) + '</a></li>');

                pager.append(item);

            }

            //bind events
            self._bindPageEvents(self);
            self._refreshPaging();
        },
        _bindPageEvents: function (self) {
            $(".pagination>li>a").click(function (event) {
                event.preventDefault();
                var currentActivePage = self._getPageNumber($(".pagination>li.active>a"));
                var newActivePage = self._getPageNumber($(this));

                $(".page-" + currentActivePage).removeClass("active");
                $(".page-" + newActivePage).addClass("active");
                self._refreshPaging();

                self._getData(false);
            });

            $(".previous").unbind("click").click(function (event) {
                var active = $(".pagination>li.active");

                var previous = active.prev();

                if (previous.length > 0) {
                    active.removeClass("active");
                    previous.addClass("active");
                }
                self._refreshPaging();
                self._getData(false);
            });


            $(".next").unbind("click").click(function (event) {
                var active = $(".pagination>li.active");

                var next = active.next();

                if (next.length > 0) {
                    active.removeClass("active");
                    next.addClass("active");
                }
                self._refreshPaging();
                self._getData(false);
            });
        }
    });
    $.fn.managePosts = function (options) {
        var control = new ManagePosts();
        control.init(options, this);
        return control;
    };
})(jQuery);


(function ($) {
    $.extend($.ui, { EditPost: {} });

    function EditPost() {
        this.widget = null;
        this.twitterValidation = null;
        this.allowRichText = null;
    }

    $.extend(EditPost.prototype, {
        init: function (options, control) {
            var self = this;
            this.widget = control;

            var defaults = {
                "twitterValidation": false,
                "allowRichText": true
            };

            var allOptions = $.extend(defaults, options);
            self.twitterValidation = allOptions.twitterValidation;
            self.allowRichText = allOptions.allowRichText;
            self._bindEvents();
            return this;
        },
        _bindEvents: function () {
            var self = this;
            if (self.twitterValidation == "true") {
                self._charCount();
            }
            if (self.allowRichText == "true") {
                $("#postCopy").addClass("formatted-text");

                tinymce.init({
                    selector: 'textarea.formatted-text',
                    menubar: false,
                    plugins: [
                        "link"
                    ],
                    toolbar: 'bullist numlist | link'
                });

            } else {
                $("#postCopy").addClass("unformatted-text");
            }

            $(".dropdown-menu li a").click(function () {
                $(".dropdown-menu li a").removeClass("selected-featuredimage");

                $(this).addClass("selected-featuredimage");
                var text = $(this).text();
                var button = $("#featuredImage");
                $("#featuredImageLabel").text(text);
                button.val(text);
                var imageUrl = $(this).data("imageurl");

                var newImage = '<img id="featuredImage" src="' + imageUrl + '" alt="' + text + '">';
                $("#featuredImage").replaceWith(newImage);
            });

            self._attachmentEvents();
            self._initPostNow();
            self._bindDelete();
            self._bindSave();
            self._bindPreview();
            self._bindFileUpload();
        },

        _getCopy: function () {
            var self = this;
            if (self.allowRichText == "true") {
                return tinyMCE.activeEditor.getContent({ format: 'raw' });
            } else {
                return $("#postCopy").val();
            }

        },
        _bindPreview: function () {
            var self = this;

            $("#preview").click(function (event) {
                event.preventDefault();

                var data = self._createPostModel();

                var encoded = JSON.stringify(data);

                var url = $(this).data("previewurl");
                $.ajax({
                    url: url,
                    data: encoded,
                    dataType: 'html',
                    contentType: 'application/json',
                    type: "POST",
                    cache: false,
                    success: function (results) {
                        var iframe = document.getElementById('previewIFrame');
                        iframe.contentWindow.document.open();
                        iframe.contentWindow.document.write(results);
                        $("#previewPostModal").modal("show");
                    }
                });

            });

        },
        _createPostModel: function () {
            var self = this;
            var title = $("#title").val();
            var postCopy = self._getCopy();

            var postNow = $("#postNow").is(":checked");
            var postTime = $("#postTime").val();
            var postDate = $("#postDate").val();
            var featuredContentId = $(".selected-featuredimage").data("target");
            var featuredImageUrl = $("#featuredImage").attr("src");
            if (!featuredContentId || featuredContentId == "") {
                featuredImageUrl = "";
            }

            var syndicatedContentId = $("#syndicatedContentId").val();

            var attachments = [];

            var attachmentList = $("#attachment-list>li");
            attachmentList.each(function () {
                var elementType = $(this).data("attachmenttype");
                var type = "";
                if (elementType == "attachment-link") {
                    type = "link";
                } else {
                    type = "file";
                }
                var attachmentTitle = $(this).find(".title");
                var attachmentName = attachmentTitle.find("span").text();
                var attachmentUrl = attachmentTitle.find("a").attr("href");
                var id = $(this).data("id");
                var isDeleted = $(this).data("isDeleted");
                attachments.push({ name: attachmentName, url: attachmentUrl, type: type, id: id, isDeleted: isDeleted });

            });


            var data = {
                contentText: postCopy,
                title: title,
                featuredContentId: featuredContentId,
                featuredContentUrl: featuredImageUrl,
                attachments: attachments,
                postNow: postNow,
                clientOffset: new Date().getTimezoneOffset(),
                publishDate: postDate + " " + postTime,
                syndicatedContentId: syndicatedContentId
            };
            return data;
        },
        _bindSave: function () {
            var self = this;

            $("#save").click(function (event) {
                event.preventDefault();
                
                $(this).attr("disabled", true);

                var listUrl = $(this).data("list");

                var data = self._createPostModel();

                var postNow = $("#postNow").is(":checked");
                var postDate = $("#postDate").val();

                var encoded = JSON.stringify(data);

                var url = $(this).data("posturl");
                $.ajax({
                    url: url,
                    data: encoded,
                    dataType: 'json',
                    contentType: 'application/json',
                    type: "POST",
                    success: function (results) {
                        if (postNow == true) {
                            window.location = listUrl + "?activeTab=past";
                        } else if (postDate <= Date.now()) {
                            window.location = listUrl + "?activeTab=past";
                        } else {
                            window.location = listUrl + "?activeTab=future";
                        }

                    }
                });
            });
        },
        _bindDelete: function () {
            $("#delete").click(function (event) {
                event.preventDefault();
                $(this).attr("disabled", true);

                var deleteUrl = $(this).data("deleteurl");
                var listUrl = $(this).data("list");
                $.ajax({
                    url: deleteUrl,
                    type: "DELETE",
                    success: function (data) {
                        window.location = listUrl;
                    }
                });

            });
        },
        _initPostNow: function () {
            var self = this;
            var postnow = $("#postNow");

            self._initDatePicker();
            self._initTimePicker();

            postnow.change(function () {

                var isChecked = $(this).is(":checked");

                if (isChecked == true) {
                    var postTimeVal = $("#postTime").val();

                    $("#postTime").data("orig", postTimeVal);
                    $("#postTime").val('');
                    $("#postTime").attr("disabled", '');

                    var postDateVal = $("#postDate").val();

                    $("#postDate").data("orig", postDateVal);
                    $("#postDate").val('');
                    $("#postDate").attr("disabled", '');


                } else {
                    var postTimeOrig = $("#postTime").data('orig');
                    if (!postTimeOrig) {
                        return;
                    }
                    $("#postTime").val(postTimeOrig).removeAttr("disabled");

                    var postDateOrig = $("#postDate").data('orig');
                    //if (!postDateOrig) {
                    //    return;
                    //}
                    $("#postDate").val(postDateOrig).removeAttr("disabled");
                }
            });

            postnow.trigger("change");
        },
        _initDatePicker: function () {
            $('.datepicker-container').DatePicker({});
        },
        _initTimePicker: function () {
            $('.timepicker-container').TimePicker();

            $("#postTime").change(function () {
                // $("#error-message-wrapper").hide();
            });
        },
        _isPostNow: function () {
            var postnow = $("#postnow");

            if (postnow.is(":checked")) {
                return true;
            } else {
                return false;
            }
        },
        _bindFileUpload: function () {
            var self = this;

            var maxFileSize = 26214400;
            var acceptFileTypes = "/(\.|\/)(gif|jpe?g|png|tiff|bmp|pdf)$/i";

            $('#fileupload').fileupload({
                url: '/admin/uploadfile',
                acceptFileTypes: acceptFileTypes,
                maxFileSize: maxFileSize,
                filesContainer: "#attachment-list",
                dataType: 'json',
                prependFiles: false,
                autoUpload: true,
                always: function(e, data) {
                    $("#attachmentName").val('');
                    var id = data.result.queuedcontentid;
                    $("#syndicatedContentId").val(id);

                    var last = $("#attachment-list>li").last();
                    last.find(".delete").click(function(event) {
                        event.preventDefault();
                        self._deleteAttachment($(this));
                    });
                    self._showHideAttachments();

                }
            });

        },
        _attachmentEvents: function () {
            var self = this;
            self._showHideAttachments();

            $(".delete").click(function (event) {
                event.preventDefault();
                self._deleteAttachment($(this));
            });



            var attachmentTemplate = $("#attachment-template");

            $("#attachLink").click(function (event) {
                var name = $("#attachmentName").val();

                if (name === "") {
                    $("#attachmentName").focus().parent().addClass("has-error");
                    return;
                } else {
                    $("#attachmentName").parent().removeClass("has-error");
                }

                var linkurl = $("#attachmentLink").val();
                if (linkurl === "") {
                    $("#attachmentLink").focus().parent().addClass("has-error");
                    return;
                } else {
                    $("#attachmentLink").parent().removeClass("has-error");
                }

                var totalAllowed = $("#attachments").data('totalallowed');
                if (totalAllowed == $("#attachment-list>li").length) {
                    return;
                }
                $("#attachmentName").val('');
                $("#attachmentLink").val('');

                var newAttachment = attachmentTemplate.clone();
                var title = newAttachment.find(".title");
                if (name === "") {
                    title.find("span").text(linkurl);
                } else {
                    title.find("span").text(name);
                }

                var link = title.find("a");
                link.attr("href", linkurl);
                link.text(linkurl);

                newAttachment.appendTo("#attachment-list").show();
                $("#attachments").show();

                var totalAttached = $("#attachment-list>li").length;

                if (totalAttached == totalAllowed) {
                    $("#attachment-panel").addClass("has-attachment");
                }

                var tweetField = $('#postCopy');

                newAttachment.find(".delete").click(function (event) {
                    event.preventDefault();
                    self._deleteAttachment($(this));
                    self._checkCharLength(tweetField);
                });


                self._checkCharLength(tweetField);

            });
        },
        _deleteAttachment: function (element) {
            var self = this;

            var li = $(element).closest("li");
            var id = li.data("id");
            if (!id) {
                li.remove();
            } else {
                li.data("isDeleted", true);
                li.hide().addClass("deleted");
            }


            self._showHideAttachments();
        },
        _showHideAttachments: function () {
            var count = $("#attachment-list>li:not(.deleted)").length;
            var totalAllowed = $("#attachments").data('totalallowed');

            if (count == 0) {
                $("#attachments").hide();
                $("#attachment-panel").removeClass("has-attachment");
                return;
            }
            else if (count < totalAllowed) {
                $("#attachment-panel").removeClass("has-attachment");
            }
            else if (count == totalAllowed) {
                $("#attachment-panel").addClass("has-attachment");
            }
            $("#attachments").show();
        },
        _checkCharLength: function (element) {

            var text = $(element).val();

            var urls = twttr.txt.extractUrlsWithIndices(text);
            var replacement22 = "0123456789012345678901";
            var replacement20 = "01234567890123456789";
            $(urls).each(function () {
                text = text.replace(this.url, replacement22);
            });

            if ($("#attachment-list>li").length > 0) {
                text += replacement22;
            }

            var count = text.length;
            var currentCountElement = $('.count');
            currentCountElement.text(count);
        },
        _charCount: function () {
            var self = this;

            var tweetChange = function () {
                self._checkCharLength($(this));
            };

            var currentCountElement = $('.count');
            currentCountElement.text('0');

            var tweetField = $('#postCopy');
            tweetField.bind('input propertychange', tweetChange);
            self._checkCharLength(tweetField);
        },

    });
    $.fn.editPost = function (options) {
        var control = new EditPost();
        control.init(options, this);
        return control;
    };
})(jQuery);