FROM alpine:latest

LABEL Name=finger

WORKDIR /app

CMD ["echo","database started"]