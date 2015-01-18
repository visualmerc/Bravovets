//resourcetopics
(function ($) {
    $.extend($.ui, { ResourceTopics: {} });

    function ResourceTopics() {
        this.widget = null;
        this.widgetSortBy = null;
        this.widgetFilterBy = null;
        this.widgetList = null;
        this.defaultSort = null;
        this.sort = null;
        this.filter = null;
    }

    $.extend(ResourceTopics.prototype, {
        init: function (options, control) {
            var self = this;
            this.widget = control;
            this.widgetSortBy = self.widget.find("div.resources-sortby");
            this.widgetFilterBy = self.widget.find("ul.resources-filterby");
            this.widgetList = self.widget.find("ul.syndicatedcontent-resources");

            var defaults = {};

            var allOptions = $.extend(defaults, options);

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
                self.widget.find("#pageResources").show();
                self._getTopics(null, self);
            });
        },
        _bindPaging: function () {
            var self = this;

            var nextPage = self.widget.find("#pageResources");
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
                self.widget.find("#pageResources").show();
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


        },
        _hide: function (item, self) {
            var hide = false;

            if (item.hasClass("hide-item")) {
                hide = true;
            }

            var syndicatedContentId = item.attr("href");

            $.ajax({
                url: "/HideContent",
                data: syndicatedContentId + "&hide=" + hide,
                type: "POST",
                success: function () {

                    item.closest("li").delay(300).fadeOut(function () { $(this).remove(); });

                }
            });
        },
        _markFavorite: function (item, self) {
            var isFavorite = true;

            if (item.hasClass("btn-removefavorite")) {
                isFavorite = false;
            }

            var syndicatedContentId = item.attr("href");

            $.ajax({
                url: "/MarkContentAsFavorite",
                data: syndicatedContentId + "&isFavorite=" + isFavorite,
                type: "POST",
                success: function () {
                    if (isFavorite) {
                        item.removeClass("btn-addfavorite").addClass("btn-removefavorite");
                        return;
                    }
                    item.removeClass("btn-removefavorite").addClass("btn-addfavorite");
                }
            });

        },
        _getTopics: function (paging, self) {

            var sortBy = self.sort;
            var filterBy = self.filter;
            var cloneLoading = self.widgetList.find(".resourcesLoading").clone();
            if (paging === null) {
                self.widgetList.find("li").remove();
            }

            self.widgetList.append(cloneLoading);
            cloneLoading.show();

            $.ajax({
                url: "/ResourceTopicsItems",
                data: "sortBy=" + sortBy + "&" + paging + "&" + filterBy,
                type: "GET",
                success: function (data) {
                    var response = $("<div>" + data + "</div");
                    var newItems = response.find("li");
                    self.widgetList.append(newItems);
                    cloneLoading.remove();

                    var paging = response.find("#pageResources");

                    var nextPage = self.widget.find("#pageResources");

                    if (paging.length === 0) {
                        nextPage.hide();
                    } else {
                        var newPage = paging.data("nextpage");

                        nextPage.data("nextpage", newPage);
                    }

                    self._bindItems();

                }
            });

        }
    });
    $.fn.resourceTopics = function (options) {
        var control = new ResourceTopics();
        control.init(options, this);
        return control;
    };
})(jQuery);
