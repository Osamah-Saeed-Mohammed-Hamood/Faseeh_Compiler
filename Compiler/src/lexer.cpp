#include "lexer.h"
#include <cctype>

Lexer::Lexer(const std::string& source) : source(source) {}

char Lexer::currentChar() {
    if (position >= source.length()) {
        return '\0';
    }
    return source[position];
}

void Lexer::advance() {
    if (currentChar() == '\n') {
        line++;
    }
    position++;
}

Token Lexer::readNumber() {
    int start = position;
    while (std::isdigit(currentChar())) {
        advance();
    }
    return { TokenType::NUMBER_LITERAL, source.substr(start, position - start), line };
}

Token Lexer::readIdentifierOrKeyword() {
    int start = position;
    while (std::isalnum(currentChar())) {
        advance();
    }
    std::string value = source.substr(start, position - start);

    if (value == "عدد") return { TokenType::KEYWORD_NUMBER, value, line };
    if (value == "اطبع") return { TokenType::KEYWORD_PRINT, value, line };

    return { TokenType::IDENTIFIER, value, line };
}

std::vector<Token> Lexer::tokenize() {
    std::vector<Token> tokens;
    while (currentChar() != '\0') {
        if (std::isspace(currentChar())) {
            advance();
            continue;
        }
        if (std::isdigit(currentChar())) {
            tokens.push_back(readNumber());
            continue;
        }
        if (std::isalpha(currentChar())) {
            tokens.push_back(readIdentifierOrKeyword());
            continue;
        }
        if (currentChar() == '=') {
            tokens.push_back({ TokenType::EQUAL, "=", line });
            advance();
            continue;
        }
        if (currentChar() == ';') {
            tokens.push_back({ TokenType::SEMICOLON, ";", line });
            advance();
            continue;
        }
        // رمز غير معروف
        tokens.push_back({ TokenType::UNKNOWN, std::string(1, currentChar()), line });
        advance();
    }
    tokens.push_back({ TokenType::END_OF_FILE, "EOF", line });
    return tokens;
}