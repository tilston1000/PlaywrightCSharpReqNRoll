Feature: Shopping

  Scenario: Add item to cart
    Given I open the application
    When I login with valid credentials
    And I add a product to the cart
    Then the cart should contain the product