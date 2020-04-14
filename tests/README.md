Testing Esquire backend with pyresttest
=======================================

## Mac OS Install (via homebrew):
1. Install pyenv and pyenv-virtualenv
```sh
    $ brew update
    $ brew install pyenv
    $ brew install pyenv-virtualenv
```
2. Create virtual environment for pyresttest, using python 3.6.4 and activate it
```sh
  $ pyenv install 3.6.4
  $ pyenv global 3.6.4
  $ pyenv virtualenv 3.6.4 pyresttest-env
  $ pyenv activate pyresttest-env
```
3. Install pycurl version that uses openssl
```sh
  $ pip3 install --compile --install-option="--with-openssl" pycurl
```
3.1 If you get 'openssl/opensslv.h' file not found when installing pycurl, add the following to your bash_profile:
```text
    export CPPFLAGS=-I/usr/local/opt/openssl/include
    export LDFLAGS=-L/usr/local/opt/openssl/lib
```
4. Install pyresttest
```sh
pip3 install pyresttest
```
5. Install JMESPath
```sh
pip3 install jmespath
```
6. Run esquire backend through Rider or Docker via command line.
```sh
docker-compose -f docker-compose.yml up
```
7. Execute tests.
```sh
  $ pyresttest http://localhost:{port} test/esquire-test.yaml
```

See https://github.com/svanoort/pyresttest/tree/master/examples for examples on
to write tests.
