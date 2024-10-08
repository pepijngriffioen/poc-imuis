build:
	docker build -t imuis-demo -f Dockerfile .

run:
	docker run -it -e CONTAINER_COMMAND="dotnet imuis-example.dll REPLACE_PARTNER_KEY REPLACE_OMGEVINGSCODE" imuis-demo
