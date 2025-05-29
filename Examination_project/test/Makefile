all: Out.txt test_plot.png
	@bash -c ' \
		echo "Super, nu har du lavet det🎉 Vil du fejre det med et spil? Tryk y for ja, eller n for nej"; \
		read answer; \
		if [ "$$answer" = "y" ]; then \
			./success_shooter.sh "🎯 SKYD AL TEKSTEN NED FOR AT VINDE 🎉"; \
		else \
			echo "Okay, ingen spil denne gang 😊"; \
		fi \
	'

Out.txt: test.exe
	mono test.exe > Out.txt

test.exe: test.cs
	mcs test.cs -out:test.exe

test_plot.png: test.txt test_plot.gnu
	gnuplot test_plot.gnu

clean:
	rm -f test.exe Out.txt test.txt test_plot.png
