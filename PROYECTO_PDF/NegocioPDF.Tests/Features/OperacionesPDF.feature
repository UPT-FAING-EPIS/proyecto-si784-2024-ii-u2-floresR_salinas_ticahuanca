Feature: Operaciones con PDFs
  Como usuario del sistema
  Quiero poder realizar operaciones con PDFs
  Según mi tipo de suscripción

  Scenario: Usuario básico realiza operación dentro del límite
    Given un usuario con id 1 y suscripción "basico"
    When intento realizar una operación "conversion"
    Then la operación debería realizarse correctamente

  Scenario: Usuario básico excede límite de operaciones
    Given un usuario con id 1 y suscripción "basico"
    When intento realizar una operación "conversion"
    And intento realizar una operación "conversion"
    And intento realizar una operación "conversion"
    And intento realizar una operación "conversion"
    And intento realizar una operación "conversion"
    And intento realizar una operación "conversion"
    Then la operación debería ser rechazada

  Scenario: Usuario premium realiza operaciones sin límite
    Given un usuario con id 2 y suscripción "premium"
    When intento realizar una operación "conversion"
    Then la operación debería realizarse correctamente