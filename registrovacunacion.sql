-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 06-11-2025 a las 03:18:15
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.0.30

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `registrovacunacion`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `agentessanitarios`
--

CREATE TABLE `agentessanitarios` (
  `ID_Agente` int(11) NOT NULL,
  `Nombre` varchar(100) NOT NULL,
  `Apellido` varchar(100) NOT NULL,
  `Dni` varchar(20) NOT NULL,
  `Matricula` varchar(50) NOT NULL,
  `Telefono` varchar(50) DEFAULT NULL,
  `ID_Usuario` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `alumnos`
--

CREATE TABLE `alumnos` (
  `ID_Alumno` int(11) NOT NULL,
  `Nombre` varchar(100) NOT NULL,
  `Apellido` varchar(100) NOT NULL,
  `Dni` varchar(20) NOT NULL,
  `TelefonoTutor` varchar(50) DEFAULT NULL,
  `FechaNacimiento` date DEFAULT NULL,
  `Grado` varchar(50) DEFAULT NULL,
  `ID_Escuela` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `escuelas`
--

CREATE TABLE `escuelas` (
  `ID_Escuela` int(11) NOT NULL,
  `Nombre` varchar(255) NOT NULL,
  `Direccion` varchar(255) DEFAULT NULL,
  `Fotos` varchar(500) DEFAULT NULL,
  `TelefonoInstitucional` varchar(50) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `registrosvacunacion`
--

CREATE TABLE `registrosvacunacion` (
  `ID_Registro` int(11) NOT NULL,
  `ID_Alumno` int(11) NOT NULL,
  `ID_Vacuna` int(11) NOT NULL,
  `ID_Agente` int(11) NOT NULL,
  `FechaAplicacion` datetime NOT NULL,
  `Dosis` varchar(50) DEFAULT NULL,
  `Observaciones` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuarios`
--

CREATE TABLE `usuarios` (
  `ID_Usuario` int(11) NOT NULL,
  `Email` varchar(255) NOT NULL,
  `PasswordHash` varchar(500) NOT NULL,
  `AvatarURL` varchar(500) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `vacunas`
--

CREATE TABLE `vacunas` (
  `ID_Vacuna` int(11) NOT NULL,
  `Nombre` varchar(100) NOT NULL,
  `Lote` varchar(100) DEFAULT NULL,
  `Descripcion` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `agentessanitarios`
--
ALTER TABLE `agentessanitarios`
  ADD PRIMARY KEY (`ID_Agente`),
  ADD UNIQUE KEY `Dni` (`Dni`),
  ADD UNIQUE KEY `Matricula` (`Matricula`),
  ADD UNIQUE KEY `ID_Usuario` (`ID_Usuario`);

--
-- Indices de la tabla `alumnos`
--
ALTER TABLE `alumnos`
  ADD PRIMARY KEY (`ID_Alumno`),
  ADD UNIQUE KEY `Dni` (`Dni`),
  ADD KEY `ID_Escuela` (`ID_Escuela`);

--
-- Indices de la tabla `escuelas`
--
ALTER TABLE `escuelas`
  ADD PRIMARY KEY (`ID_Escuela`);

--
-- Indices de la tabla `registrosvacunacion`
--
ALTER TABLE `registrosvacunacion`
  ADD PRIMARY KEY (`ID_Registro`),
  ADD KEY `ID_Alumno` (`ID_Alumno`),
  ADD KEY `ID_Vacuna` (`ID_Vacuna`),
  ADD KEY `ID_Agente` (`ID_Agente`);

--
-- Indices de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  ADD PRIMARY KEY (`ID_Usuario`),
  ADD UNIQUE KEY `Email` (`Email`);

--
-- Indices de la tabla `vacunas`
--
ALTER TABLE `vacunas`
  ADD PRIMARY KEY (`ID_Vacuna`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `agentessanitarios`
--
ALTER TABLE `agentessanitarios`
  MODIFY `ID_Agente` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `alumnos`
--
ALTER TABLE `alumnos`
  MODIFY `ID_Alumno` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `escuelas`
--
ALTER TABLE `escuelas`
  MODIFY `ID_Escuela` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `registrosvacunacion`
--
ALTER TABLE `registrosvacunacion`
  MODIFY `ID_Registro` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  MODIFY `ID_Usuario` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `vacunas`
--
ALTER TABLE `vacunas`
  MODIFY `ID_Vacuna` int(11) NOT NULL AUTO_INCREMENT;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `agentessanitarios`
--
ALTER TABLE `agentessanitarios`
  ADD CONSTRAINT `FK_Agente_Usuario` FOREIGN KEY (`ID_Usuario`) REFERENCES `usuarios` (`ID_Usuario`);

--
-- Filtros para la tabla `alumnos`
--
ALTER TABLE `alumnos`
  ADD CONSTRAINT `alumnos_ibfk_1` FOREIGN KEY (`ID_Escuela`) REFERENCES `escuelas` (`ID_Escuela`);

--
-- Filtros para la tabla `registrosvacunacion`
--
ALTER TABLE `registrosvacunacion`
  ADD CONSTRAINT `registrosvacunacion_ibfk_1` FOREIGN KEY (`ID_Alumno`) REFERENCES `alumnos` (`ID_Alumno`),
  ADD CONSTRAINT `registrosvacunacion_ibfk_2` FOREIGN KEY (`ID_Vacuna`) REFERENCES `vacunas` (`ID_Vacuna`),
  ADD CONSTRAINT `registrosvacunacion_ibfk_3` FOREIGN KEY (`ID_Agente`) REFERENCES `agentessanitarios` (`ID_Agente`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
