import unittest
from preparar_png import sin_leyendas


class PreparacionTest(unittest.TestCase):
    def test_omite_leyenda_sin_tocar_interacciones_y_tipos(self):
        for body in ('Alice -> Bob: hola', 'class Cuenta', 'actor Usuario\nUsuario --> (Ingresar)'):
            source = '@startuml\n' + body + '\nlegend right\nExplicacion\nendlegend\n@enduml\n'
            self.assertEqual('@startuml\n' + body + '\n@enduml\n', sin_leyendas(source))

    def test_preserva_includes_y_texto_con_palabra_legend(self):
        source = '@startuml\n!include ../tema.puml\nAlice -> Bob: legend\n@enduml\n'
        self.assertEqual(source, sin_leyendas(source))

    def test_rechaza_cierre_faltante(self):
        with self.assertRaisesRegex(ValueError, 'sin cierre'):
            sin_leyendas('@startuml\nlegend\ntexto\n@enduml\n')

    def test_rechaza_leyenda_anidada(self):
        with self.assertRaisesRegex(ValueError, 'anidado'):
            sin_leyendas('legend\nlegend\nendlegend\nendlegend\n')


if __name__ == '__main__':
    unittest.main()
