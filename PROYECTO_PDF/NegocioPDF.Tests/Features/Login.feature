Feature: Login de Usuario
  Como usuario del sistema
  Quiero poder iniciar sesión
  Para acceder a las funcionalidades del sistema

  Scenario: Login exitoso
    Given un usuario con correo "test@test.com" y contraseña "password123"
    When intento iniciar sesión
    Then debería iniciar sesión correctamente

  Scenario: Login fallido con credenciales inválidas
    Given un usuario con correo "invalid@test.com" y contraseña "wrongpass"
    When intento iniciar sesión
    Then debería ver un mensaje de error

  Scenario: Login fallido con correo vacío
    Given un usuario con correo "" y contraseña "password123"
    When intento iniciar sesión
    Then debería ver un mensaje de error
    