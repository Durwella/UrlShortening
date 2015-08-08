/// <reference path="../typings/servicemodel.d.ts" />
/// <reference path="../typings/angularjs/angular.d.ts" />

module UrlShortenerApp {
    "use strict";
    import ShortUrlResponse = Durwella.UrlShortening.Web.ServiceModel.ShortUrlResponse;
    import ShortUrlRequest = Durwella.UrlShortening.Web.ServiceModel.ShortUrlRequest;
    import ResponseStatus = Durwella.UrlShortening.Web.ServiceModel.ResponseStatus;

    interface IUrlShortenerScope extends ng.IScope {
        longUrl: string;
        customPath: string;
        shortenedUrl: string;
        errorMessage: string;
        waiting: boolean;
        submitLongUrl: Function;
    }

    class UrlShortenerCtrl {

        // $inject annotation.
        // It provides $injector with information about dependencies to be injected into constructor
        // it is better to have it close to the constructor, because the parameters must match in count and type.
        // See http://docs.angularjs.org/guide/di
        static $inject = [
            "$scope",
            "$http",
            "$location"
        ];

        // dependencies are injected via AngularJS $injector
        // controller's name is registered in Application.ts and specified from ng-controller attribute in index.html
        constructor(
            private $scope: IUrlShortenerScope,
            private $http: ng.IHttpService,
            private $location: ng.ILocationService
        ) {
            $scope.submitLongUrl = () => {
                if (!$scope.longUrl)
                    return;
                $scope.shortenedUrl = null;
                $scope.errorMessage = null;
                $scope.waiting = true;
                $http.post("/shorten", <ShortUrlRequest> { Url: $scope.longUrl, CustomPath: $scope.customPath })
                    .success((response: ShortUrlResponse) => {
                        $scope.shortenedUrl = response.Shortened;
                        $scope.waiting = false;
                    })
                    .error((response: any) => {
                        console.log("Error: ", response);
                        var message = "Unknown Error";
                        if (response.hasOwnProperty("ResponseStatus")) {
                            var responseStatus = <ResponseStatus>response.ResponseStatus;
                            message = responseStatus.Message;
                        }
                        $scope.errorMessage = message;
                        $scope.waiting = false;
                    });
            };
        }
    }

    angular.module("urlShortenerApp", [])
        .controller("urlShortenerCtrl", UrlShortenerCtrl);
}