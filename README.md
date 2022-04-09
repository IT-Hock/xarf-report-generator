<div id="top"></div>

[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]
[![LinkedIn][linkedin-shield]][linkedin-url]

<!-- PROJECT LOGO -->
<br />
<div align="center">
  <a href="https://github.com/IT-Hock/xarf-report-generator">
    <img src="images/logo.png" alt="Logo" width="80" height="80">
  </a>

<h3 align="center">XARF Report Generator by IT-Hock</h3>

  <p align="center">
    Generates <a href="https://github.com/abusix/xarf">XARF</a> reports from multiple sources.
    <br />
    <a href="https://github.com/IT-Hock/xarf-report-generator/wiki"><strong>Explore the docs »</strong></a>
    <br />
    <br />
    <a href="https://github.com/IT-Hock/xarf-report-generator/issues">Report Bug</a>
    ·
    <a href="https://github.com/IT-Hock/xarf-report-generator/issues">Request Feature</a>
  </p>
</div>

<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgments">Acknowledgments</a></li>
  </ol>
</details>

<!-- ABOUT THE PROJECT -->
## About The Project

[![XARF Report Generator Screen Shot][product-screenshot]](https://github.com/It-Hock/xarf-report-generator)

XARF Report Generator is a command line tool that generates <a href="https://github.com/abusix/xarf">XARF</a> reports from multiple sources.
Currently it supports the following sources:

- Windows EventViewer (Failed Logon Attempts)
- IPBan (Recent Bans)

<p align="right">(<a href="#top">back to top</a>)</p>

<!-- USAGE EXAMPLES -->
## Usage

Get logs between two dates:

```
xarf-report-generator -s 2022-04-09T00:00:00Z -e 2022-04-08T00:00:00Z
```

Filter by IP list file:

```
xarf-report-generator -f ip-list.txt
```

Change output path:

```
xarf-report-generator -o /path/to/output/dir
```

For more examples, please refer to the [Wiki](https://github.com/IT-Hock/xarf-report-generator/wiki)

<p align="right">(<a href="#top">back to top</a>)</p>

<!-- ROADMAP -->
## Roadmap

- [ ] (Maybe) Automatically report to AbuseIPDB.com
- [ ] IIS Logs collector

See the [open issues](https://github.com/IT-Hock/xarf-report-generator/issues) for a full list of proposed features (and known issues).

<p align="right">(<a href="#top">back to top</a>)</p>

<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

If you have a suggestion that would make this better, please fork the repo and create a pull request. You can also simply open an issue with the tag "enhancement".
Don't forget to give the project a star! Thanks again!

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

<p align="right">(<a href="#top">back to top</a>)</p>

<!-- LICENSE -->
## License

Distributed under the  GNU AFFERO GENERAL PUBLIC LICENSE v3. See `LICENSE` for more information.

<p align="right">(<a href="#top">back to top</a>)</p>


<!-- CONTACT -->
## Contact

IT-Hock - info@it-hock.de

Project Link: [https://github.com/IT-Hock/xarf-report-generator](https://github.com/IT-Hock/xarf-report-generator)

<p align="right">(<a href="#top">back to top</a>)</p>

<!-- ACKNOWLEDGMENTS -->
## Acknowledgments

* [IT-Hock](https://it-hock.de)

<p align="right">(<a href="#top">back to top</a>)</p>

<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/IT-Hock/xarf-report-generator.svg?style=for-the-badge
[contributors-url]: https://github.com/IT-Hock/xarf-report-generator/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/IT-Hock/xarf-report-generator.svg?style=for-the-badge
[forks-url]: https://github.com/IT-Hock/xarf-report-generator/network/members
[stars-shield]: https://img.shields.io/github/stars/IT-Hock/xarf-report-generator.svg?style=for-the-badge
[stars-url]: https://github.com/IT-Hock/xarf-report-generator/stargazers
[issues-shield]: https://img.shields.io/github/issues/IT-Hock/xarf-report-generator.svg?style=for-the-badge
[issues-url]: https://github.com/IT-Hock/xarf-report-generator/issues
[license-shield]: https://img.shields.io/github/license/IT-Hock/xarf-report-generator.svg?style=for-the-badge
[license-url]: https://github.com/IT-Hock/xarf-report-generator/blob/master/LICENSE.txt
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://linkedin.com/in/dominic-hock
[product-screenshot]: images/screenshot.png
