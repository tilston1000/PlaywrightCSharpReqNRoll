@smoke
Feature: Shopping

  Scenario Outline: Add <product> to cart
    Given I open the application
    When I add a "<product>" to the cart
    Then the cart should contain "<product>"

    Examples:
      | product                     |
      | Sauce Labs Backpack         |
      | Sauce Labs Bike Light       |

  Scenario: Cart perists after logout and login
    Given I open the application
    When I add a "Sauce Labs Backpack" to the cart
    And I log out of the application
    And I login to the application
    And I click on the cart icon
    Then the cart should contain "Sauce Labs Backpack"