//trendingtopics

$(function () {
    $.fn.TweetLength = function (objectName, settings) {
        var $parent, config;
        this.settings = settings;
        var MAX_TWEET_LENGTH = 140;

        $parent = $(this);
        if (typeof config === "undefined" || config === null) {
            config = {};
        }

        config.myName = objectName;
        if (this.settings != null) {
            jQuery.extend(config, this.settings);
        }

        this.each(function (index) {
            var $me;
            $me = $(this);
            var _tweetField = null;
            var _tweetFieldID = null;
            var _count = 0;
            var _currentCountElement = null;
            var _remainingCountElement = null;
            var _messageElement = null;
            var _prefixElement = null;
            var _prefixAttachmentElement = null;
            var _suffixElement = null;

            var _selectText = function (start, end) {
                var e = document.getElementById(_tweetFieldID); // I don't know why... but $(this) don't want to work today :-/
                if (!e) return;
                else if (e.setSelectionRange) {
                    e.focus();
                    e.setSelectionRange(start, end);
                } /* WebKit */
                else if (e.createTextRange) {
                    var range = e.createTextRange();
                    range.collapse(true);
                    range.moveEnd('character', end);
                    range.moveStart('character', start);
                    range.select();
                } /* IE */
                else if (e.selectionStart) {
                    e.selectionStart = start;
                    e.selectionEnd = end;
                }
            };

            var checkLength = function (element) {

                var text = $(element).val();
                var maxLength = MAX_TWEET_LENGTH;

                var urls = twttr.txt.extractUrlsWithIndices(text);
                var replacement22 = "0123456789012345678901";
                var replacement20 = "01234567890123456789";
                $(urls).each(function () {
                    text = text.replace(this.url, replacement22);
                    maxLength -= replacement22.length;
                });

                if ($('#urlAttachment').length > 0) {
                    maxLength -= replacement22.length;
                }

                if ($(".post-image").length > 0) {
                    text += replacement20;
                    maxLength -= replacement20.length;
                }

                _count = text.length;
                _currentCountElement.text(_count);
                _remainingCountElement.text(maxLength);
                var prefixText = _prefixElement.text();
                if (maxLength < MAX_TWEET_LENGTH)
                    prefixText = _prefixAttachmentElement.text();
                _messageElement.text(prefixText + ' ' + maxLength + ' ' + _suffixElement.text());
                if (_count > maxLength) {
                    $me.addClass('error');
                    tweet = $(element).val();
                } else {
                    $me.removeClass('error');
                }
            };


            var _tweetChange = function () {
                checkLength($(this));
            };

            var _init = function () {
                _currentCountElement = $me.find('.current-count');
                _remainingCountElement = $me.find('.remaining-count');
                _currentCountElement.text('0');
                _messageElement = $me.find('.message');
                _prefixElement = $me.find('.twitter-prefix');
                _prefixAttachmentElement = $me.find('.twitter-attachment-prefix');
                _suffixElement = $me.find('.twitter-suffix');
                _tweetFieldID = $me.attr('data-tweetfield');
                if (_tweetFieldID) {
                    _tweetField = $('#' + $me.attr('data-tweetfield'));
                    _tweetField.bind('input propertychange', _tweetChange);
                    checkLength(_tweetField);
                }
            };
            return _init();
        });
        return this;
    };

});

$(function () {
    $.fn.TimePicker = function (objectName, settings) {
        var $parent, config;
        this.settings = settings;

        $parent = $(this);
        if (typeof config === "undefined" || config === null) {
            config = {};
        }

        config.myName = objectName;
        if (this.settings != null) {
            jQuery.extend(config, this.settings);
        }

        this.each(function (index) {
            var $me;
            $me = $(this);
            var _language = "en";

            var init = function () {
                if ($me.attr('data-language')) {
                    _language = $me.attr('data-language');
                }
                $me.find('.bootstrap-timepicker input').timepicker({showMinutes: false, lowerCaseMeridian: true, showMeridian: false });
               
            };
            return init();
        });
        return this;
    };

});

$(function () {
    $.fn.DatePicker = function (objectName, settings) {
        var $parent, config;
        this.settings = settings;
        $parent = $(this);
        if (typeof config === "undefined" || config === null) {
            config = {};
        }

        config.myName = objectName;
        if (this.settings != null) {
            jQuery.extend(config, this.settings);
        }

        this.each(function (index) {
            var $me;
            $me = $(this);
            var language = "en";
            
            var format = "M dd, yyyy";
            var initialDate = $me.find('.input-group.date input').data('current');
            var currentDate =$me.find('.input-group.date input').data('current-date');
            var startDate = "-0d";
            if (initialDate < currentDate) {
                startDate = "";
            }
            var init = function () {
                if ($me.attr('data-language')) {
                    language = $me.attr('data-language');
                }
                if ($me.attr('data-format')) {
                    format = $me.attr('data-format');
                }
                var datepicker = $me.find('.date').datepicker({
                    todayBtn: true,
                    todayHighlight: true,
                    language: language,
                    format: format,
                    startDate:startDate
                });
                $me.data('datepicker', datepicker);
                // If there is a date in the input box, set the internal date of the datepicker
                if (initialDate) {
                    $me.find('.date').datepicker('setDate', initialDate);
                }

              
                return datepicker;
            };
            return init();
        });
        return this;
    };

});

(function ($) {
    $.extend($.ui, { TrendingTopics: {} });

    function TrendingTopics() {
        this.widget = null;
        this.widgetSortBy = null;
        this.widgetFilterBy = null;
        this.widgetList = null;
        this.defaultSort = null;
        this.sort = null;
        this.filter = null;
        this.getUrl = null;
    }

    $.extend(TrendingTopics.prototype, {
        init: function (options, control) {
            var self = this;
            this.widget = control;

            var defaults = {
                //sortByName: "div.trending-topics-sortby",
                //filterByName: "ul.trending-topics-filterby",
                //widgetListName: "ul.syndicatedcontent-trendingtopics",
                //getUrl: "/TrendingTopicItems"
            };

            var allOptions = $.extend(defaults, options);

            this.getUrl = allOptions.getUrl;
            this.widgetSortBy = self.widget.find(allOptions.sortByName);
            this.widgetFilterBy = self.widget.find(allOptions.filterByName);
            this.widgetList = self.widget.find(allOptions.widgetListName);

            self._bindPaging();
            self._bindSort();
            self._bindFilter();
            self._bindItems();

            return this;
        },
        _bindFilter: function () {
            var self = this;
            self.filter = self.widgetFilterBy.find("li.active > a").attr("href");

            self.widgetFilterBy.find("li > a").click(function (event) {
                event.preventDefault();

                self.widgetFilterBy.find("li.active").removeClass("active");
                $(this).parent().addClass("active");
                self.filter = $(this).attr("href");
                self.widget.find("#nextPage").show();
                self._getTopics(null, self);
            });
        },
        _bindPaging: function () {
            var self = this;

            var nextPage = self.widget.find("#nextPage");
            self.defaultSort = nextPage.attr("href");

            nextPage.click(function (event) {
                event.preventDefault();

                var paging = $(this).data("nextpage");

                self._getTopics(paging, self);
            });

        },
        _bindSort: function () {
            var self = this;
            this.widgetSortBy.find(".customedropdown").bravoVetDropDown();
            self.sort = self.widgetSortBy.find("select").val();

            self.widgetSortBy.find("select").change(function (event) {

                self.sort = $(this).val();
                self.widget.find("#nextPage").show();
                self._getTopics(null, self);

            });
        },
        _bindItems: function () {

            var self = this;

            self.widgetList.find(".favorite").click(function (event) {
                event.preventDefault();
                self._markFavorite($(this), self);
            });

            self.widgetList.find(".showHide").click(function (event) {
                event.preventDefault();
                self._hide($(this), self);
            });

            self.widgetList.find(".share").socialPostDialog().remove();
            self.widgetList.find(".share").socialPostDialog({ "dialogType": "trendingtopics","fileupload":false,afterPost:self._afterShare });
        },
        _afterShare:function(element) {
            element.closest("li").delay(300).fadeOut(function () {
                $(this).remove();
                var items = $(".syndicatedcontent-trendingtopics > li");
                if (items.length <= 2) {
                    $('.trending-topics-filterby').find("li.active > a").trigger("click");
                    //var filter = $("#show-favorites-filter");
                    //var filterText = filter.text().toLowerCase();
                    //var noFilterItems = $(".noItems");
                    //var orig_text = $(noFilterItems).html();
                    //var changedText = orig_text.format(filterText);

                    //$(noFilterItems).html(changedText).show();
                    //$("#nextPage").hide();
                }
            });
        },
        _hide: function (item, self) {
            var hide = false;

            if (item.hasClass("hide-item")) {
                hide = true;
            }

            var syndicatedContentId = item.attr("href");
            var activeTab = self.widgetFilterBy.find("li.active > a");
            $.ajax({
                url: "/HideContent",
                data: syndicatedContentId + "&hide=" + hide,
                type: "POST",
                cache: false,
                success: function () {
                    item.closest("li").delay(300).fadeOut(function () {

                        $(this).remove();

                       // var filter = $("#show-hidden-filter");

                        var items = self.widgetList.find("li");

                        if (items.length <= 2) {
                            self.widgetFilterBy.find("li.active > a").trigger("click");
                        }
                    });
                }
            });
        },
        _markFavorite: function (item, self) {
            var isFavorite = true;

            if (item.hasClass("btn-removefavorite")) {
                isFavorite = false;
            }

            var syndicatedContentId = item.attr("href");
            var activeTab = self.widgetFilterBy.find("li.active > a");
            var activeTabFilter = activeTab.attr("href");

            $.ajax({
                url: "/MarkContentAsFavorite",
                data: syndicatedContentId + "&isFavorite=" + isFavorite,
                type: "POST",
                cache: false,
                success: function () {
                    if (isFavorite) {
                        item.removeClass("btn-addfavorite").addClass("btn-removefavorite");
                        return;
                    } else {
                        if (activeTabFilter == "filterBy=Favorites") {
                            item.closest("li").delay(300).fadeOut(function () {

                                $(this).remove();

                                var filter = $("#show-favorites-filter");

                                var items = self.widgetList.find("li");

                                if (items.length <= 2) {
                                    self.widgetFilterBy.find("li.active > a").trigger("click");
                                }
                            });
                        }
                    }

                    item.removeClass("btn-removefavorite").addClass("btn-addfavorite");
                }
            });

        },
        _getTopics: function (paging, self) {

            var sortBy = self.sort;
            var filterBy = self.filter;
            var cloneLoading = self.widgetList.find(".loading");
            if (paging === null) {
                self.widgetList.find("li").remove();
            }

            self.widgetList.append(cloneLoading);
            cloneLoading.show();
            var nextPage = self.widget.find("#nextPage");
            nextPage.hide();


            $.ajax({
                url: self.getUrl,
                data: "sortBy=" + sortBy + "&" + paging + "&" + filterBy,
                type: "GET",
                cache: false,
                success: function (data) {
                    var response = $("<div>" + data + "</div");
                    var newItems = response.find("li");
                    self.widgetList.append(newItems);
                    cloneLoading.remove();

                    var paging = response.find("#nextPage");

                    if (paging.length === 0) {
                        //nextPage.hide();
                    } else {

                        var newPage = paging.data("nextpage");

                        nextPage.data("nextpage", newPage);
                        nextPage.show();
                    }

                    self._bindItems();

                }
            });

        }
    });
    $.fn.trendingTopics = function (options) {
        var control = new TrendingTopics();
        control.init(options, this);
        return control;
    };
})(jQuery);


