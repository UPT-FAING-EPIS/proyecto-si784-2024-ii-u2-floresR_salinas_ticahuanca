Feature: Registro de Usuario
  Como visitante del sistema
  Quiero poder registrarme
  Para convertirme en usuario del sistema

  Scenario: Registro exitoso
    Given un nuevo usuario con nombre "Nuevo Usuario", correo "nuevo@test.com" y contraseña "newpass123"
    When intento registrar el usuario
    Then el usuario debería registrarse correctamente

  Scenario: Registro fallido con correo existente
    Given un nuevo usuario con nombre "Usuario Duplicado", correo "test@test.com" y contraseña "pass123"
    When intento registrar el usuario
    Then debería ver un mensaje de error de registro

  Scenario: Registro fallido con datos incompletos
    Given un nuevo usuario con nombre "", correo "" y contraseña ""
    When intento registrar el usuario
    Then debería ver un mensaje de error de registro