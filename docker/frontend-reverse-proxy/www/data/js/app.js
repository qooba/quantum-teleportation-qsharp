angular.module('app', ['components'])

  .controller('IndexController', function ($scope, $locale, $http) {

    $scope.teleport = function () {
      $http({
        method: 'GET',
        url: '/quantum/teleportation?text='+$scope.q
      }).then(function successCallback(response) {
        console.log(response);
        $scope.t = response.data.TeleportedText
        $scope.qnum = response.data.QubitsCount
      }, function errorCallback(response) {
        console.log(response);
      });
      //alert("Indexing started ... please wait");
    };


  });